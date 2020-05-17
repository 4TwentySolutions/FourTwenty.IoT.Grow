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
using Microsoft.Extensions.Logging;
using Quartz.Impl;

#if !DebugLocalWin
using FourTwenty.IoT.Server.Components.Relays;
using FourTwenty.IoT.Server.Components.Sensors;
#endif

namespace GrowIoT.Services
{
    //TODO rename to something like IotRuntimeService or similar
    public class IoTConfigService : IIoTConfigService, IDisposable
    {
        private IList<IModule> _modules;

        private readonly ILogger<IoTConfigService> _logger;
        private GpioController _gpioController;

        public IoTConfigService(ILogger<IoTConfigService> logger)
        {
            _logger = logger;
#if !DebugLocalWin
            _gpioController = new GpioController(PinNumberingScheme.Logical);
#endif
        }


        public IList<IModule> GetModules()
        {
            return _modules;
        }

        public GpioController Gpio => _gpioController;

        public bool IsInitialized { get; private set; }

        public async ValueTask Initialize(GrowBoxViewModel box)
        {

            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            if (box?.Modules == null)
                return;

            _modules = new List<IModule>();

            foreach (var module in box.Modules)
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
#if DebugLocalWin
                    mod = new MockModule(rules, new[] { module.Pins.FirstOrDefault() })
                    {
                        Id = module.Id,
                        Name = nameof(MockModule)
                    };
#else
                    mod = new DhtSensor(module.Pins.FirstOrDefault(), _gpioController, rules)
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
                        mod = new Relay(module.Pins, _gpioController)
                        {
                            Id = module.Id,
                            Name = module.Name,
                            Rules = rules
                        };
                    }
#endif
                }

                if (mod != null)
                {
                    //if (mod is ISensor sensor)
                    //    sensor.DataReceived += SensorOnDataReceived;
                    //if (mod is IRelay relay)
                    //    relay.StateChanged += RelayOnStateChanged;
                    _modules.Add(mod);
                }
            }

            IsInitialized = true;
        }

        private void RelayOnStateChanged(object? sender, RelayEventArgs e)
        {

        }

        private void SensorOnDataReceived(object? sender, SensorEventArgs e)
        {

        }

        public IModule GetModule(int id)
        {
            return _modules.FirstOrDefault(d => d.Id == id);
        }

        #region IDisposable Support
        private bool _disposedValue; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _gpioController?.Dispose();
                _gpioController = null;
            }
            _disposedValue = true;
        }
        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
