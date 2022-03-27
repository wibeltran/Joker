using Elastic.Apm.AspNetCore;
using Joker.Configuration;
using Joker.Logging;
using Joker.WebApp.Extensions;
using Joker.WebApp.Hubs;
using Serilog;

var configuration = JokerConfigurationHelper.GetConfiguration();
Log.Logger = LoggerBuilder.CreateLoggerElasticSearch(x =>
{
    x.Url = configuration["elk:url"];
    x.BasicAuthEnabled = false;
    x.IndexFormat = "joker-logs";
    x.AppName = "Joker.WebApp";
    x.Enabled = true;
    x.Configuration = configuration;
    x.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
});

try
{
    var builder = WebApplication.CreateBuilder(args);

    var services = builder.Services;

    services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();
    services.AddAuthorization();
    services.AddHttpClient();
    services.AddHttpContextAccessor();
    services.AddJokerGatewayApiClient(configuration);
    services.AddJokerIdentityApiClient(configuration);
    services.AddJokerAuthentication(configuration);
    services.AddApiServices();
    services.AddUserServices();
    services.AddJokerEventBus(configuration);
    services.AddDataProtection();
    services.AddSignalR();
    services.AddJokerOpenTelemetry(configuration);

    builder.Host.UseSerilog();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseElasticApm(configuration);
    app.UseStaticFiles();
    app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
    app.UseRouting();
    app.UseForwardedHeaders();
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapHub<CampaignCreatedNotificationHub>("/campaignCreated");
    });

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "The application failed to start correctly");
}
finally
{
    Log.CloseAndFlush();
}