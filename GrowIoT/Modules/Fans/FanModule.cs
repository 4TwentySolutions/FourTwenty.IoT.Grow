using System;
using System.Collections.Generic;
using System.Device.Gpio;
using GrowIoT.Enums;
using GrowIoT.Interfaces;

namespace GrowIoT.Modules.Fans
{
    public class FanModule : BaseModule
    {
        private readonly int _gpioPin;

        public FanModule(string name, int? gpioPin = null, List<ModuleRule> rules = null) : base(rules, name)
        {
            _gpioPin = gpioPin ?? 0;
            Type = ModuleType.Fan;
        }

        public override void Init(GpioController controller)
        {
            base.Init(controller);

            if (_gpioPin > 0)
            {
                Controller.OpenPin(_gpioPin);

                Pins = new List<int>()
                {
                    _gpioPin
                };
            }
        }
    }
}
