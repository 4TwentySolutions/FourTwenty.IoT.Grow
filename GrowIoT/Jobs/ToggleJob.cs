﻿using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Entities;
using GrowIoT.Modules;
using GrowIoT.Modules.Relays;
using Quartz;

namespace GrowIoT.Jobs
{
    public class ToggleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var module = (IoTBaseModule)dataMap["module"];
            var rule = (ModuleRuleDto)dataMap["rule"];

            if (module != null && rule != null)
            {
                int? gpioPin = null;
                /* if (!string.IsNullOrEmpty(rule.ModuleName))
                 {
                     if (module is TwoRelayModule twoRelay)
                     {
                         gpioPin = twoRelay.SubModules.FirstOrDefault(x => x.Value.Name == rule.ModuleName).Key;
                     }
                 }
                 else
                 {
                     gpioPin = module.Pins.FirstOrDefault();
                 }
                 */
                gpioPin = module.Pins.FirstOrDefault();

                module.Toggle(gpioPin);
            }

            return Task.CompletedTask;
        }
    }
}
