using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Modules;

namespace GrowIoT.Modules.Relays
{
    public class TwoRelayModule : IoTBaseModule
    {
        public Dictionary<int, IoTBaseModule> SubModules { get; set; } = new Dictionary<int, IoTBaseModule>();

        public TwoRelayModule(string name, int firstGpioPin, int secondGpioPin, List<ModuleRule> rules = null) : base(name, null, rules)
        {
            Pins = new List<int>
            {
                firstGpioPin,
                secondGpioPin
            };

            Type = ModuleType.TwoRelay;
        }

        public override void Init(GpioController controller)
        {
            base.Init(controller);

            if (Pins.Any())
                Controller.SetPinMode(Pins.FirstOrDefault(), PinMode.Output);

            if (Pins.Count > 1)
            {
                Controller.OpenPin(Pins.LastOrDefault());
                Controller.SetPinMode(Pins.LastOrDefault(), PinMode.Output);
            }

            foreach (var subModule in SubModules)
            {
                subModule.Value.Init(controller);
            }
        }

        public TwoRelayModule AddSubModule(IoTBaseModule subModule, int gpioPin)
        {
            SubModules.Add(gpioPin, subModule);
            return this;
        }

    }
}
