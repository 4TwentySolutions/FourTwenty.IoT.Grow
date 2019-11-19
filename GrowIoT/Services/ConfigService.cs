using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GrowIoT.Enums;
using GrowIoT.Models;
using GrowIoT.Modules;
using GrowIoT.Modules.Fans;
using GrowIoT.Modules.Light;
using GrowIoT.Modules.Relays;
using GrowIoT.Modules.Sensors;
using Newtonsoft.Json;

namespace GrowIoT.Services
{
    public class ConfigService
    {
        private ConfigModel _currentConfig;

        public async Task<ConfigModel> GetConfig()
        {
            return GetDefaultConfig();
            /*
            ConfigModel config = null;
            
            try
            {
                string storageFolderPath = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(storageFolderPath, "config.txt");
                Debug.WriteLine($"--- Config path: {filePath}\n");
                var content = string.Empty;
                if (!File.Exists(filePath))
                {
                    config = GetDefaultConfig();
                    using (var fs = File.Create(filePath))
                    {
                        var info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(config));
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    content = await File.ReadAllTextAsync(filePath);
                    //config = GetDefaultConfig(controller);
                    config = JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n--- !!! Exception !!! ---\n");
                Console.WriteLine($"--- {e.Message} ---\n");
                config = GetDefaultConfig();
            }

            return _currentConfig = config;*/
        }

        public void InitConfig(GpioController controller, ConfigModel config = null)
        {
            if (config != null)
            {
                _currentConfig = config;
            }

            if (_currentConfig == null)
                return;

            foreach (var module in _currentConfig.Modules)
            {
                module.Init(controller);
            }
        }

        public ConfigModel GetDefaultConfig()
        {
            Console.WriteLine("--- Using default config ---");
            var lightName = "Box Light";
            var fanName = "Fan";
            var dhtName = "Dht Sensor";
            var twoRelayName = "Light&Fan Relay";

            var dhtPin = 4;
            var fanPin = 27;
            var lightPin = 17;

            return new ConfigModel
            {
                ListeningPort = 8001,
                Modules = new List<BaseModule>
                {
                    new DhtSensor(dhtName,dhtPin,new List<ModuleRule>
                    {
                        new ModuleRule
                        {
                            Type = JobType.Read,
                            CronExpression = "0/10 0/1 * 1/1 * ? *"
                        }
                    }),
                    new TwoRelayModule(twoRelayName,lightPin,fanPin,new List<ModuleRule>()
                        {
                            new ModuleRule
                            {
                                ModuleName = lightName,
                                Type = JobType.On,
                                CronExpression = "0/1 0/1 7-19 ? * *"
                            },
                            new ModuleRule
                            {
                                ModuleName = lightName,
                                Type = JobType.Off,
                                CronExpression = "0/1 0/1 20-6 ? * *"
                            },
                            //new ModuleRule
                            //{
                            //    ModuleName = fanName,
                            //    Type = JobType.On,
                            //    CronExpression = "0/1 0-20 * ? * *"
                            //},
                            //new ModuleRule
                            //{
                            //    ModuleName = fanName,
                            //    Type = JobType.Off,
                            //    CronExpression = "0/1 21-59 * ? * *"
                            //}
                        })
                        .AddSubModule(new LightModule(lightName),lightPin)
                        //.AddSubModule(new FanModule(fanName),17)
                }
            };
        }
    }
}
