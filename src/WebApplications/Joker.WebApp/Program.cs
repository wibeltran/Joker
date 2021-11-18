using Joker.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Joker.WebApp;

public class Program
{
    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        Log.Logger = CreateSerilogLogger(configuration, "Joker.WebApp");

        try
        {
            Log.Information("Application starting up...");

            CreateHostBuilder(args)
                .Build()
                .Run();
                    
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "The application failed to start correctly.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Creates Host Builder
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging(k=>k.ClearProviders())
            .UseSerilog();

    /// <summary>
    /// Returns configuration with environment
    /// </summary>
    /// <returns></returns>
    private static IConfiguration GetConfiguration()
    {
        string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables();

        return builder.Build();
    }

    public static ILogger CreateSerilogLogger(IConfiguration configuration, string applicationName)
    {
        return LoggerBuilder.CreateLoggerElasticSearch(x =>
        {
            x.Url = configuration["elk:url"];
            x.BasicAuthEnabled = false;
            x.IndexFormat = "joker-logs";
            x.AppName = applicationName;
            x.Enabled = true;
        });
    }
}