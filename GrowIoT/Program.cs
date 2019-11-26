using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Connect.Modules;
using GrowIoT.Extensions;
using GrowIoT.Interfaces;
using GrowIoT.Jobs;
using GrowIoT.Services;
using Quartz;
using Quartz.Impl;

namespace GrowIoT
{
    class Program
    {
        private static IIotConfigService _configService;
        private static IList<BaseModule> _modules;
        private static IoTNetworkService _networkService;
        private static IScheduler _scheduler;

        public static async Task Main(string[] args)
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            Console.WriteLine($"--- Scheduler:{ _scheduler != null } ---");
            _configService = new IoTConfigService();

            using (GpioController controller = new GpioController(PinNumberingScheme.Logical))
            {
                Console.WriteLine("--- Start init ---");
                var config = await _configService.GetConfig();
                _modules = config.Modules;
                _configService.InitConfig(controller, config);
                _networkService = new IoTNetworkService(config.ListeningPort);
                Console.WriteLine("--- End init ---");
                await StartJobs();

                var curIp = IpExtensions.GetCurrentIp();
                if (curIp != null)
                {
                    _networkService.PackReceive += NetworkServiceOnPackReceive;
                    _networkService.StartNetwork(curIp, config.ListeningPort);                    
                }

                Console.ReadLine();
            }
        }

        private static void NetworkServiceOnPackReceive(GrowPackage package)
        {
            Debug.WriteLine("--- NetworkServiceOnPackReceive ---");
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
