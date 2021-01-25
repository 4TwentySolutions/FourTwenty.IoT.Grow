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
using System.Device.Gpio.Drivers;
using FourTwenty.IoT.Server.Components.Sensors;
using FourTwenty.IoT.Server.Components.Relays;
using GrowIoT.Extensions;
using Quartz;
using Quartz.Impl.Matchers;
#if !DebugLocalWin
using FourTwenty.IoT.Server.Components.Relays;
using FourTwenty.IoT.Server.Components.Sensors;
#endif

namespace GrowIoT.Services
{
    public class IoTRuntimeService : IIoTConfigService, IDisposable
    {
        private IList<IModule> _modules;

        private readonly ILogger<IoTRuntimeService> _logger;
        private GpioController _gpioController;

        public IoTRuntimeService(ILogger<IoTRuntimeService> logger)
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
                            Period = TimeSpan.FromSeconds(ruleData.Delay.GetValueOrDefault()),
                            Pin = moduleRule.Pin,
                            ModuleId = module.Id,
                            IsEnabled = moduleRule.IsEnabled,
                            Id = moduleRule.Id
                        };

                        rules.Add(cronRule);
                    }
                }

                //TODO create helper or service that will be handle the modules creation process
                //mod = new MockModule(rules, new[] { module.Pins.FirstOrDefault() });
                //mod = new MockRelay(rules, module.Pins);
                #region Module creation

                switch (module.Type)
                {
                    case ModuleType.HumidityAndTemperature:
                        mod = new DhtSensor(module.Pins.FirstOrDefault(), _gpioController);
                        break;
                    case ModuleType.Relay:
                        mod = new Relay(module.Pins, _gpioController);
                        break;
                    case ModuleType.RangeFinder:
                        mod = new RangeFinderSensor(module.Pins.FirstOrDefault(), module.Pins.LastOrDefault(), _gpioController);
                        break;
                }

                if (mod != null)
                {
                    mod.Id = module.Id;
                    mod.Name = module.Name;
                    mod.Rules = rules;

                    _modules.Add(mod);
                }

                #endregion

            }

            IsInitialized = true;
        }

        public void UpdateMuuleRuleState(int moduleId, int ruleId, bool isEnabled)
		{
			if (moduleId > 0 && ruleId > 0)
			{
				var module = _modules.FirstOrDefault(d => d.Id == moduleId);

				if (module != null)
				{
					var rule = module.Rules.FirstOrDefault(x => x.Id == ruleId);
					if (rule != null)
					{
						rule.IsEnabled = isEnabled;
					}
				}
			}
		}

        public IModule GetModule(int id)
        {
            return _modules.FirstOrDefault(d => d.Id == id);
        }

        public async Task ControlModuleJobs(int moduleId, WorkState workState)
        {
	        var module = _modules.FirstOrDefault(d => d.Id == moduleId);
	        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
	        if (module != null)
	        {
		        switch (workState)
		        {
			        case WorkState.Running:
				        if (module.RulesWorkState == WorkState.Stopped)
				        {
					       module.Rules?.Execute();
				        }
				        else
				        {
					        await scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(moduleId.ToString()));
				        }
				        break;
			        case WorkState.Paused:
				        await scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(moduleId.ToString()));
				        break;
			        case WorkState.Stopped:
				        var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupEquals(moduleId.ToString()));
				        await scheduler.DeleteJobs(jobKeys);
				        break;
		        }

		        module.RulesWorkState = workState;
	        }
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
