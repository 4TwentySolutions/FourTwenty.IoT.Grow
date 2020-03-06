using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Server.Components;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Server;
using FourTwenty.IoT.Server.Components.Relays;
using FourTwenty.IoT.Server.Components.Sensors;
using FourTwenty.IoT.Server.Jobs;
using FourTwenty.IoT.Server.Rules;
using GrowIoT.Interfaces;
using GrowIoT.Models.Tests;
using GrowIoT.ViewModels;
using Newtonsoft.Json;
using Quartz;
using Iot.Device.DHTxx;

namespace GrowIoT.Services
{
    public class IoTConfigService : IIoTConfigService
    {
        private IList<IModule> _modules;

        public void InitConfig(IScheduler scheduler, GrowBoxViewModel config, GpioController controller = null)
        {
            if (config?.Modules == null)
                return;

            _modules = new List<IModule>();

            foreach (var module in config.Modules)
            {
                IoTComponent mod = null;
                var rules = new List<IRule>();

                foreach (var moduleRule in module.Rules)
                {
                    if (moduleRule.RuleType == RuleType.CronRule)
                    {
                        var ruleData = JsonConvert.DeserializeObject<CronRuleData>(moduleRule.RuleContent) ?? new CronRuleData();

                        var cronRule = new CronRule(ruleData.Job, ruleData.CronExpression, scheduler)
                        {
                            Period = TimeSpan.FromSeconds(ruleData.Delay.GetHashCode()),
                            Pin = moduleRule.Pin
                        };

                        rules.Add(cronRule);
                    }
                }


                if (module.Type == ModuleType.HumidityAndTemperature)
                {

                    //#if DebugLocalWin

                    //                    mod = new MockModule(rules, new[] { module.Pins.FirstOrDefault() })
                    //                    {
                    //                        Name = nameof(MockModule)
                    //                    };

                    //#else
                    //                mod = new DhtSensor(module.Pins.FirstOrDefault(), controller, rules)
                    //                    {
                    //                        Id = module.Id,
                    //                        Name = module.Name
                    //                    };
                    //#endif

                    mod = new DhtSensor(module.Pins.FirstOrDefault(), controller, rules)
                    {
                        Id = module.Id,
                        Name = module.Name
                    };

                }

                if (module.Type == ModuleType.TwoRelay)
                {
                    if (module.Pins?.Length >= 2)
                    {
                        mod = new DoubleRelay(module.Pins[0], module.Pins[1], controller)
                        {
                            Id = module.Id,
                            Name = module.Name,
                            Rules = rules
                        };
                    }
                }

                if (mod is ISensor sens)
                {
                    sens.DataReceived += SensOnDataReceived;
                }

                _modules.Add(mod);
            }
        }

        private void SensOnDataReceived(object? sender, SensorEventArgs e)
        {
            if (sender is IoTComponent component)
            {
                Console.WriteLine($@"{component.Name} - {e.Data}");

            }

        }

        public ConfigDto GetConfig()
        {
            return null;
        }

        public IList<IModule> GetModules()
        {
            return _modules;
        }

        public async Task<long> UpdateConfig(ConfigDto model)
        {
            try
            {
                model.CurrentVersion = DateTime.Now.Ticks;
                var filePath = Constants.Constants.ConfigPath;
                await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(model));
                return model.CurrentVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
    }
}
