﻿using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Interfaces;
using FourTwenty.IoT.Server.Components;
using FourTwenty.IoT.Server.Components.Sensors;
using FourTwenty.IoT.Server.Rules;
using GrowIoT.Interfaces;
using GrowIoT.ViewModels;
using Newtonsoft.Json;

namespace GrowIoT.Services
{
    public class IoTConfigService : IIoTConfigService
    {
        private ConfigDto _currentConfig;
        // private GpioController _gpioController;
        private IList<IoTComponent> _modules;

        public async Task<ConfigDto> LoadConfig()
        {
            ConfigDto config;

            try
            {
                var filePath = Constants.Constants.ConfigPath;
                if (!File.Exists(filePath))
                {
                    config = new ConfigDto();
                    await using var fs = File.Create(filePath);
                    var info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(config));
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
                else
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    config = JsonConvert.DeserializeObject<ConfigDto>(content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n--- !!! Exception !!! ---\n");
                Console.WriteLine($"--- {e.Message} ---\n");
                config = new ConfigDto();
            }
            return _currentConfig = config;
        }

        public void InitConfig(GpioController controller = null, GrowBoxViewModel config = null)
        {
            if (_currentConfig?.Modules == null)
                return;

            _modules = new List<IoTComponent>();

            foreach (var module in _currentConfig.Modules)
            {
                IoTComponent mod = null;
                var rules = new List<IRule>();
                foreach (var moduleRuleDto in module.Rules)
                {
                    var cronRule = new CronRule(moduleRuleDto.Type, moduleRuleDto.CronExpression)
                    {
                        Period = TimeSpan.FromSeconds(moduleRuleDto.Period)
                    };

                    rules.Add(cronRule);
                }
                if (module.Type == ModuleType.HumidityAndTemperature)
                {
                    mod = new DhtSensor(module.Pin.GetValueOrDefault(), controller, rules);
                }

                _modules.Add(mod);
            }
        }

        public ConfigDto GetConfig()
        {
            return _currentConfig;
        }

        public IList<IoTComponent> GetModules()
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
