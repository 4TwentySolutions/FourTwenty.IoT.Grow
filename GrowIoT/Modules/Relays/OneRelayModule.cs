using System.Collections.Generic;
using System.Device.Gpio;
using GrowIoT.Enums;
using GrowIoT.Interfaces;

namespace GrowIoT.Modules.Relays
{
    public class OneRelayModule : BaseModule
    {
        private readonly int _gpioPin;

        public OneRelayModule(int gpioPin)
        {
            _gpioPin = gpioPin;
        }

        public OneRelayModule(int gpioPin, List<ModuleRule> rules) : base(rules)
        {
            _gpioPin = gpioPin;
        }

        public override void Init(GpioController controller)
        {
            base.Init(controller);

            Controller.OpenPin(_gpioPin);
            Pins = new List<int>
            {
                _gpioPin
            };
            Type = ModuleType.Relay;
        }
    }
}
