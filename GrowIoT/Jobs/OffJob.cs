using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Modules;
using GrowIoT.Modules;
using GrowIoT.Modules.Relays;
using Quartz;

namespace GrowIoT.Jobs
{
    public class OffJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var module = (IoTBaseModule)dataMap["module"];
            var rule = (ModuleRule)dataMap["rule"];

            if (module != null && rule != null)
            {
                int? gpioPin = null;
                if (!string.IsNullOrEmpty(rule.ModuleName))
                {
                    if (module is TwoRelayModule twoRelay)
                    {
                        var mod = twoRelay.SubModules.FirstOrDefault(x => x.Value.Name == rule.ModuleName);
                        if (mod.Key > 0)
                            gpioPin = mod.Key;
                    }
                }
                else
                {
                    gpioPin = module.Pins.FirstOrDefault();
                }

                Console.WriteLine($"--- {module.Name} -> High ---");

                module.SetValue(PinValue.High, gpioPin);
            }

            return Task.CompletedTask;
        }
    }
}
