using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Models.Config;
using GrowIoT.Interfaces;
using GrowIoT.Modules;
using Newtonsoft.Json;

namespace GrowIoT.Services
{
    public class IoTConfigService : IIotConfigService
    {
        private ConfigModel _currentConfig;

        public void InitConfig(GpioController controller, ConfigModel config = null)
        {
            if (config != null)
            {
                _currentConfig = config;
            }

            if (_currentConfig == null || _currentConfig.Modules == null)
                return;

            foreach (var module in _currentConfig.Modules)
            {
                if (module is IoTBaseModule mod)
                    mod.Init(controller);
            }
        }

        public async Task<ConfigModel> GetConfig()
        {
            
            ConfigModel config;
            
            try
            {
                string storageFolderPath = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(storageFolderPath, "config.txt");
                Debug.WriteLine($"--- Config path: {filePath}\n");
                if (!File.Exists(filePath))
                {
                    config = new ConfigModel();
                    using (var fs = File.Create(filePath))
                    {
                        var info = new UTF8Encoding(true).GetBytes(JsonConvert.SerializeObject(config));
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }
                else
                {
                    var content = await File.ReadAllTextAsync(filePath);
                    config = JsonConvert.DeserializeObject<ConfigModel>(content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n--- !!! Exception !!! ---\n");
                Console.WriteLine($"--- {e.Message} ---\n");
                config = new ConfigModel();
            }
            return _currentConfig = config;
        }
    }
}
