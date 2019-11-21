﻿using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrowIoT.Modules;
using GrowIoT.Modules.Relays;
using Quartz;


namespace GrowIoT.Jobs
{
    public class OnJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var module = (BaseModule)dataMap["module"];
            var rule = (ModuleRule)dataMap["rule"];

            if (module != null && rule != null)
            {
                int? gpioPin = null;
                if (!string.IsNullOrEmpty(rule.ModuleName))
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

                Console.WriteLine($"--- {module.Name} -> Low ---");
                module.SetValue(PinValue.Low, gpioPin);
            }

            return Task.CompletedTask;
        }
    }
}
