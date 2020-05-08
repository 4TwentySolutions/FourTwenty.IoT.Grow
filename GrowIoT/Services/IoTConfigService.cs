using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Server.Components;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Server.Rules;
using GrowIoT.Interfaces;
using GrowIoT.Models.Tests;
using GrowIoT.ViewModels;
using Newtonsoft.Json;
using Quartz;
using Microsoft.Extensions.Logging;
using Infrastructure.Interfaces;
using Infrastructure.Entities;
using Microsoft.Extensions.DependencyInjection;

#if !DebugLocalWin
using System.IO;
using FourTwenty.IoT.Server;
using FourTwenty.IoT.Server.Components.Relays;
using FourTwenty.IoT.Server.Components.Sensors;
using FourTwenty.IoT.Server.Jobs;
using Iot.Device.DHTxx;
#endif

namespace GrowIoT.Services
{
    public class IoTConfigService : IIoTConfigService
    {
        private IList<IModule> _modules;
        private readonly ILogger<IoTConfigService> _logger;

        public bool IsInitialized { get; set; }

        public IoTConfigService(ILogger<IoTConfigService> logger)
        {
            _logger = logger;
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
                //var filePath = Constants.Constants.ConfigPath;
                // await File.WriteAllTextAsync(filePath, JsonConvert.SerializeObject(model));
                return model.CurrentVersion;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public void Initialize(InitializableOptions options)
        {
            if (options is ConfigInitOptions configInitOptions)
            {
                if (configInitOptions.Config?.Modules == null)
                    return;

                _modules = new List<IModule>();

                foreach (var module in configInitOptions.Config.Modules)
                {
                    IoTComponent mod = null;
                    var rules = new List<IRule>();

                    foreach (var moduleRule in module.Rules)
                    {
                        if (moduleRule.RuleType == RuleType.CronRule)
                        {
                            var ruleData = JsonConvert.DeserializeObject<CronRuleData>(moduleRule.RuleContent) ?? new CronRuleData();

                            var cronRule = new CronRule(ruleData.Job, ruleData.CronExpression, configInitOptions.Scheduler)
                            {
                                Period = TimeSpan.FromSeconds(ruleData.Delay.GetHashCode()),
                                Pin = moduleRule.Pin
                            };

                            rules.Add(cronRule);
                        }
                    }


                    if (module.Type == ModuleType.HumidityAndTemperature)
                    {
#if DebugLocalWin
                        mod = new MockModule(rules, new[] { module.Pins.FirstOrDefault() })
                        {
                            Id = module.Id,
                            Name = nameof(MockModule)
                        };
#else
                        mod = new DhtSensor(module.Pins.FirstOrDefault(), configInitOptions.Controller, rules)
                        {
                            Id = module.Id,
                            Name = module.Name
                        };
#endif

                    }

                    if (module.Type == ModuleType.Relay)
                    {
#if DebugLocalWin
                        mod = new MockRelay(rules, module.Pins)
                        {
                            Id = module.Id,
                            Name = nameof(MockRelay)
                        };
#else
                        if (module.Pins?.Length >= 2)
                        {
                            mod = new Relay(module.Pins, configInitOptions.Controller)
                            {
                                Id = module.Id,
                                Name = module.Name,
                                Rules = rules
                            };
                        }
#endif
                    }

                    if (mod != null)
                        _modules.Add(mod);
                }

                IsInitialized = true;
            }
        }

        public IModule GetModule(int id)
        {
            return _modules.FirstOrDefault(x => x.Id == id);
        }
    }

    public class ConfigInitOptions : InitializableOptions
    {
        public IScheduler Scheduler { get; set; }
        public GrowBoxViewModel Config { get; set; }
        public GpioController Controller { get; set; } = null;
    }
}
