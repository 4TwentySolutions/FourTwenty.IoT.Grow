using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
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
        private static IList<BaseModule> _modules;
        private static IScheduler _scheduler;

        public static async Task Main(string[] args)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            Console.WriteLine($"--- Scheduler:{ _scheduler != null } ---");
            _configService = new IoTConfigService();
            var config = await _configService.GetConfig();
            try
            {
                using GpioController controller = new GpioController(PinNumberingScheme.Logical);
                Console.WriteLine("--- Start init ---");
                _modules = config.Modules;
                _configService.InitConfig(controller, config);
                Console.WriteLine("--- End init ---");
                await StartJobs();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            Console.WriteLine("--- Starting Server ---");
            var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();


            var hostUrl = configuration["hosturl"];
            if (string.IsNullOrEmpty(hostUrl))
                hostUrl = "http://0.0.0.0:5000";


            return Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.UseUrls(hostUrl);
                                webBuilder.UseStartup<Startup>();
                            });
        }

        public static async Task StartJobs()
        {
            Console.WriteLine("--- Jobs starting ---");
            await _scheduler.Start();

            if (_modules != null)
            {
                foreach (var sensor in _modules)
                {
                    if (sensor.Rules != null)
                    {
                        var index = 0;
                        foreach (var moduleRule in sensor.Rules)
                        {
                            index++;
                            Type jobType = null;

                            switch (moduleRule.Type)
                            {
                                case JobType.Toggle:
                                    jobType = typeof(ToggleJob);
                                    break;
                                case JobType.Read:
                                    jobType = typeof(ReadJob);
                                    break;
                                case JobType.On:
                                    jobType = typeof(OnJob);
                                    break;
                                case JobType.Off:
                                    jobType = typeof(OffJob);
                                    break;
                                case JobType.Period:
                                    jobType = typeof(PeriodJob);
                                    break;
                            }

                            if (jobType == null)
                                return;

                            var job = JobBuilder.Create(jobType).Build();
                            var trigger = TriggerBuilder.Create()

                                .WithIdentity($"{sensor.Name}_{moduleRule.Type}_Job_{index}", moduleRule.Type.ToString())

                                .WithCronSchedule(moduleRule.CronExpression)

                                .StartNow()

                                .WithPriority(1)

                                .Build();

                            job.JobDataMap.Add("module", sensor);
                            job.JobDataMap.Add("rule", moduleRule);

                            await _scheduler.ScheduleJob(job, trigger);
                        }
                    }
                }
            }
        }
    }
}
