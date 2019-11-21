using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using GrowIoT.Enums;
using GrowIoT.Interfaces;

namespace GrowIoT.Modules.Fans
{
    public class FanModule : BaseModule
    {
        public FanModule(string name, int? gpioPin = null, List<ModuleRule> rules = null) : base(gpioPin, rules, name)
        {
            Type = ModuleType.Fan;
        }
    }
}
