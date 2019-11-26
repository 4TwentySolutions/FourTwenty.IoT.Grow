using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Interfaces.Sensors;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Connect.Modules;
using Iot.Device.DHTxx;

namespace GrowIoT.Modules.Sensors
{
    public class DhtSensor : IoTBaseModule, ISensor<ModuleResponse<DthData>>
    {
        public DhtSensor(string name, int gpioPin, List<ModuleRule> rules = null) : base(name, gpioPin, rules)
        {
            Type = ModuleType.HumidityAndTemperature;
        }

        public override void Init(GpioController controller)
        {
            base.Init(controller);

            Controller.SetPinMode(Pins.FirstOrDefault(), PinMode.Output);
        }

        public async Task<ModuleResponse<DthData>> GetData()
        {
            var result = new ModuleResponse<DthData>();
            try
            {
                var dht11 = new Dht11(Pins.FirstOrDefault(), PinNumberingScheme.Logical);

                result.Data = new DthData
                {
                    Humidity = dht11.Humidity,
                    Temperature = dht11.Temperature.Celsius
                };
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;
            }

            return result;
        }

        public override async Task<BaseModuleResponse> ReadData()
        {
            return await GetData();
        }
    }

    public class DthData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
