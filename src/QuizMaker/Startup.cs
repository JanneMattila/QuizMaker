using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using QuizMaker.Data;
using QuizMaker.Hubs;
using QuizMaker.Services;

namespace QuizMaker;

public class Startup
{
    private const string QuizAdminClaimName = "QuizAdmin";
    private const string QuizAdminTenant = "tenant";
    private const string QuizAdminUser = "user";

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    private IQuizDataContext? _quizDataContext;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim(QuizAdminClaimName, QuizAdminTenant, QuizAdminUser)
                .Build();
        });

        services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            var existingOnTokenValidatedHandler = options.Events.OnTokenValidated;
            options.Events.OnTokenValidated = async context =>
            {
                await existingOnTokenValidatedHandler(context);
                await UserValidationLogic(context);
            };
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.ConsentCookie.MaxAge = TimeSpan.FromDays(2);
        });

        var signalR = services.AddSignalR();

        // If Azure SignalR Service connection is provided
        // then use enable it and otherwise just self-host.
        var signalRConnectionString = Configuration["SignalRConnectionString"];
        if (!string.IsNullOrEmpty(signalRConnectionString))
        {
            signalR.AddAzureSignalR(signalRConnectionString);
        }

        var storageConnectionString = Configuration["StorageConnectionString"];
        _quizDataContext = new QuizDataContext(new QuizDataContextOptions()
        {
            StorageConnectionString = storageConnectionString
        });
        services.AddSingleton<IQuizDataContext>((services) => _quizDataContext);

        services.AddSingleton<ConnectionStorage>();
        services.AddSingleton<IUserIdProvider, UniqueIdentifierUserIdProvider>();
        services.AddSingleton<IHostedService, ConnectionBackgroundService>();
        services
            .AddControllersWithViews()
            .AddControllersAsServices();
        services.AddRazorPages();


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders =
                ForwardedHeaders.XForwardedHost |
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCookiePolicy();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapRazorPages();
            endpoints.MapHub<QuizHub>("/QuizHub");
            endpoints.MapHub<QuizResultsHub>("/QuizResultsHub");
        });
    }

    private async Task UserValidationLogic(TokenValidatedContext context)
    {
        ArgumentNullException.ThrowIfNull(_quizDataContext);

        if (context.Principal is null)
        {
            context.Fail("No valid user principal available.");
            return;
        }

        var identity = context.Principal.Identity as ClaimsIdentity;
        if (identity is null)
        {
            context.Fail("No valid claims identity available.");
            return;
        }

        var tenantID = context.Principal.GetTenantId();
        ArgumentNullException.ThrowIfNull(tenantID);
        var hasTenantAccess = await _quizDataContext.AdminTenantHasAccessAsync(tenantID);
        if (hasTenantAccess)
        {
            identity.AddClaim(new Claim(QuizAdminClaimName, QuizAdminTenant));
            return;
        }

        var objectID = context.Principal.GetObjectId();
        ArgumentNullException.ThrowIfNull(objectID);
        var hasUserAccess = await _quizDataContext.AdminUserHasAccessAsync(tenantID, objectID);
        if (hasUserAccess)
        {
            identity.AddClaim(new Claim(QuizAdminClaimName, QuizAdminUser));
            return;
        }

        context.Fail("No valid access configured.");

        context.Response.Redirect("/Home/NotAuthorized");
        context.HandleResponse();
    }
}
