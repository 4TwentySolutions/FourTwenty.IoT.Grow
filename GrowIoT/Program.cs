using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GrowIoT.Extensions;
using GrowIoT.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GrowIoT
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build().SeedIdentity().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            CreateLogger();
            Console.WriteLine("--- Starting Server ---");
            var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();


            var hostUrl = configuration["hosturl"];
            //if (string.IsNullOrEmpty(hostUrl))
            //    hostUrl = $"http://0.0.0.0:5000";


            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(hostUrl);
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static void CreateLogger()
        {
            var logger = LoggerProvider.GetLoggerConfiguration("Development")
                .Enrich.WithProperty("Platform", "LocalPC")
                .CreateLogger();
            Log.Logger = logger;
            Log.Information(nameof(CreateLogger));
        }
    }
}
