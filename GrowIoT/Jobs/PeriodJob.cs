using System;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Modules;
using Quartz;

namespace GrowIoT.Jobs
{
    public class PeriodJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var module = (IoTBaseModule)dataMap["module"];
            var rule = (ModuleRuleDto)dataMap["rule"];

            if (module != null && rule != null)
            {
                var gpioPin = module.Pins.FirstOrDefault();

                Console.WriteLine($"--- PeriodJob --- {module.Name} --- Period:{rule.Period} ---");
                module.SetValue(PinValue.Low, gpioPin);
                await Task.Delay(TimeSpan.FromSeconds(rule.Period));
                module.SetValue(PinValue.High, gpioPin);

            }
        }
    }
}
