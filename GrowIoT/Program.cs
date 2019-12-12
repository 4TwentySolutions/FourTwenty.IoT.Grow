using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using FourTwenty.IoT.Connect.Modules;
using GrowIoT.Interfaces;
using GrowIoT.Jobs;
using GrowIoT.Services;

namespace GrowIoT
{
    public class Program
    {
        private static IIoTConfigService _configService;

        public static async Task Main(string[] args)
        {
            _configService = new IoTConfigService();
            var config = await _configService.GetConfig();
            CreateHostBuilder(args, config.ListeningPort).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args, int port)
        {
            Console.WriteLine("--- Starting Server ---");
            var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();


            var hostUrl = configuration["hosturl"];
            if (string.IsNullOrEmpty(hostUrl))
                hostUrl = $"http://0.0.0.0:5000";


            return Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseUrls(hostUrl);
                                webBuilder.UseStartup<Startup>();
                            });
        }
    }
}
