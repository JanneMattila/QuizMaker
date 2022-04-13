using System;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetry();

        services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

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
