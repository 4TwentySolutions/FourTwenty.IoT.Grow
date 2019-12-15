using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using GrowIoT.Interfaces;
using GrowIoT.Modules;
using GrowIoT.Modules.Sensors;
using Newtonsoft.Json;

namespace GrowIoT.Services
{
    public class IoTConfigService : IIoTConfigService
    {
        private ConfigDto _currentConfig;
        private GpioController _gpioController;
        private IList<IoTBaseModule> _modules;

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

        public void InitConfig(GpioController controller = null, ConfigDto config = null)
        {
            if (controller != null)
            {
                _gpioController = controller;
            }

            if (config != null)
            {
                _currentConfig = config;
            }

            if (_currentConfig?.Modules == null)
                return;

            _modules = new List<IoTBaseModule>();

            foreach (var module in _currentConfig.Modules)
            {
                IoTBaseModule mod = null;
                if (module.Type == ModuleType.HumidityAndTemperature)
                {
                    mod = new DhtSensor(module.Name, module.Pin.GetValueOrDefault(), module.Rules);
                }

                if (_gpioController != null)
                    mod?.Init(_gpioController);

                _modules.Add(mod);
            }
        }

        public ConfigDto GetConfig()
        {
            return _currentConfig;
        }

        public IList<IoTBaseModule> GetModules()
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
