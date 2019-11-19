using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using GrowIoT.Enums;

namespace GrowIoT.Modules.Relays
{
    public class TwoRelayModule : BaseModule
    {
        public Dictionary<int, BaseModule> SubModules { get; set; } = new Dictionary<int, BaseModule>();

        public TwoRelayModule(string name, int firstGpioPin, int secondGpioPin, List<ModuleRule> rules = null) : base(rules, name)
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

            Controller.OpenPin(Pins.FirstOrDefault());

            Controller.SetPinMode(Pins.FirstOrDefault(), PinMode.Output);


            Controller.OpenPin(Pins.LastOrDefault());
            Controller.SetPinMode(Pins.LastOrDefault(), PinMode.Output);


            foreach (var subModule in SubModules)
            {
                subModule.Value.Init(controller);
            }
        }

        public TwoRelayModule AddSubModule(BaseModule subModule, int gpioPin)
        {
            SubModules.Add(gpioPin, subModule);
            return this;
        }

    }
}
