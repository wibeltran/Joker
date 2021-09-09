using System;
using System.IO;
using System.Net;
using Joker.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Subscription.Api
{
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var configuration = GetConfiguration();
            Log.Logger = CreateSerilogLogger(configuration, "Subscription.Api");

            try
            {
                Log.Information("Application starting up...");

                CreateHostBuilder(configuration, args)
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
        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var ports = GetDefinedPorts(configuration);
                    
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });

                        options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
                })
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

        public static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var isValidGrpcPort = int.TryParse(config["GRPC_PORT"], out var grpcPort);
            if (!isValidGrpcPort || grpcPort <= 0)
            {
                grpcPort = 5017;
            }
            var isValidPort = int.TryParse(config["PORT"], out var port);
            if (!isValidPort || port <= 0)
            {
                port = 5007;
            }
            
            return (port, grpcPort);
        }
    }
}