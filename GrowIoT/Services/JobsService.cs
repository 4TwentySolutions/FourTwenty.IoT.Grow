using System;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Interfaces;
using GrowIoT.Jobs;
using Quartz;
using Quartz.Impl;

namespace GrowIoT.Services
{
    public class JobsService : IJobsService
    {
        private IScheduler _scheduler;

        public async Task Init()
        {
            _scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        }

        public async Task StartJobs(ConfigDto config)
        {
            try
            {
                Console.WriteLine($"--- Jobs starting --- version: {config.CurrentVersion}");
                await _scheduler.Start();

                if (config.Modules != null)
                {
                    foreach (var sensor in config.Modules)
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task StopJobs()
        {
            try
            {
                if (_scheduler != null && _scheduler.IsStarted)
                {
                    _scheduler.Clear();
                    _scheduler.Shutdown();
                }

                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
