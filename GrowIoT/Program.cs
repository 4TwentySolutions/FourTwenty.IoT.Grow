using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using GrowIoT.Enums;
using GrowIoT.Jobs;
using GrowIoT.Modules;
using GrowIoT.Services;
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
        private static CancellationTokenSource _tokenSource;

        public static async Task Main(string[] args)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            Console.WriteLine($"--- Scheduler:{ _scheduler != null } ---");
            _configService = new ConfigService();

            using (GpioController controller = new GpioController(PinNumberingScheme.Logical))
            {
                Console.WriteLine("--- Start init ---");
                var config = await _configService.GetConfig();
                _modules = config.Modules;
                _configService.InitConfig(controller, config);
                _networkService = new NetworkService(config.ListeningPort);
                Console.WriteLine("--- End init ---");
                await StartJobs();               

                Console.ReadLine();
            }
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
                            }

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
