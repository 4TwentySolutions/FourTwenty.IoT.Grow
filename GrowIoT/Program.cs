using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;
using GrowIoT.Enums;
using GrowIoT.Jobs;
using GrowIoT.Modules;
using GrowIoT.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Quartz;
using Quartz.Impl;

namespace GrowIoT
{
    class Program
    {
        private static ConfigService _configService;
        private static IList<BaseModule> _modules;
        private static NetworkService _networkService;
        private static IScheduler _scheduler;

        public static async Task Main(string[] args)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            Console.WriteLine($"--- Scheduler:{ _scheduler != null } ---");
            _configService = new ConfigService();
            using (GpioController controller = new GpioController(PinNumberingScheme.Logical))
            {
                Console.WriteLine("--- Start init ---");
                var config = await _configService.GetConfig(controller);
                _modules = config.Modules;
                _configService.InitConfig(controller);
                _networkService = new NetworkService(config.ListeningPort);
                Console.WriteLine("--- End init ---");
                await StartJobs();

                bool isCancel = false;


                while (!isCancel)
                {
                    Console.CancelKeyPress += (object sender, ConsoleCancelEventArgs eventArgs) =>
                    {
                        isCancel = true;
                        controller.Dispose();
                        Console.WriteLine("Pin cleanup complete!");
                    };
                }
            }


            //BuildWebHost(args).Run();
        }

        public static async Task StartJobs()
        {
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
                            }

                            var job = JobBuilder.Create(jobType).Build();
                            ITrigger trigger = TriggerBuilder.Create()

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

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:5001")
                .UseStartup<Startup>()
                .Build();
    }
}
