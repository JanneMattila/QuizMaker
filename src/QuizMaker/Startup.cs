using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using QuizMaker.Data;
using QuizMaker.Hubs;
using QuizMaker.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizMaker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Instead of using the default validation (validating against a single issuer value, as we do in
                    // line of business apps), we inject our own multitenant validation logic
                    ValidateIssuer = true,

                    // If the app is meant to be accessed by entire organizations, add your issuer validation logic here.
                    IssuerValidator = (issuer, securityToken, validationParameters) =>
                    {
                        return IssuerValidationLogic(issuer) ? issuer : null;
                    }
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnTicketReceived = context =>
                    {
                        // If your authentication logic is based on users then add your logic here
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.Redirect("/Error");
                        context.HandleResponse(); // Suppress the exception
                        return Task.CompletedTask;
                    },
                    // If your application needs to authenticate single users, add your user validation below.
                    OnTokenValidated = context =>
                    {
                        return UserValidationLogic(context.Principal);
                    }
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
            services.AddSingleton<IQuizDataContext>((services) =>
            new QuizDataContext(new QuizDataContextOptions()
            {
                StorageConnectionString = storageConnectionString
            }));

            services.AddSingleton<ConnectionStorage>();
            services.AddSingleton<IUserIdProvider, UniqueIdentifierUserIdProvider>();
            services.AddSingleton<IHostedService, ConnectionBackgroundService>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        private bool IssuerValidationLogic(string issuer)
        {
            return true;
        }

        private Task UserValidationLogic(ClaimsPrincipal principal)
        {
            return Task.CompletedTask;
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
    }
}
