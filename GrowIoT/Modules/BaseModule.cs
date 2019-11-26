using System.Collections.Generic;
using System.Device.Gpio;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Models;
using FourTwenty.IoT.Connect.Modules;
using GrowIoT.Interfaces;
using Newtonsoft.Json;

namespace GrowIoT.Modules
{
    public class IoTBaseModule : BaseModule, IIoTModule
    {
        [JsonIgnore]
        public GpioController Controller;


        public IoTBaseModule(string name, int? gpio, List<ModuleRule> rules) : base(gpio, rules, name)
        {

        }

        public virtual void Init(GpioController controller)
        {
            Debug.WriteLine($"--- Module {Name}: Initialized ---");

            if (Pins == null || !Pins.Any())
                return;

            Controller = controller;

            Controller.OpenPin(Pins.FirstOrDefault());
        }

        public virtual Task<BaseModuleResponse> ReadData()
        {
            return Task.FromResult(new BaseModuleResponse()
            {
                IsSuccess = true,
                Message = "base module result"
            });
        }

        /// <summary>
        /// Toggle module gpio value
        /// </summary>
        /// <param name="gpioPin">pin number</param>
        public virtual void Toggle(int? gpioPin = null)
        {
            if (Pins != null && Pins.Count > 0)
            {
                var relayPin = gpioPin.HasValue ? Pins.FirstOrDefault(x => x == gpioPin) : Pins.FirstOrDefault();

                if (relayPin > 0)
                {
                    var val = Controller.Read(relayPin);
                    val = val == PinValue.High ? PinValue.Low : PinValue.High;
                    Debug.WriteLine($"--- Relay {relayPin}: set to {val.ToString()}");
                    Controller.SetPinMode(relayPin, PinMode.Input);
                    Controller.Write(relayPin, val);
                    Controller.SetPinMode(relayPin, PinMode.Output);
                }
            }
        }

        /// <summary>
        /// Set module gpio value (Low - on , High - off)
        /// </summary>
        /// <param name="value">Low - on , High - off</param>
        /// <param name="gpioPin">pin number</param>
        public virtual void SetValue(PinValue value, int? gpioPin = null)
        {
            if (Pins != null && Pins.Count > 0)
            {
                var relayPin = gpioPin.HasValue ? Pins.FirstOrDefault(x => x == gpioPin) : Pins.FirstOrDefault();

                if (relayPin > 0)
                {
                    // old logic

                    // Controller.SetPinMode(relayPin, PinMode.Input);
                    // var curValue = Controller.Read(relayPin);
                    // Debug.WriteLine($"--- Pin {relayPin}: value is {curValue.ToString()}");
                    // if (curValue == value)
                    //     return;
                    //Debug.WriteLine($"--- Pin {relayPin}: set to {value.ToString()}");

                    Controller.SetPinMode(relayPin, PinMode.Output);
                    Controller.Write(relayPin, value);
                }
            }

        }
    }
}
