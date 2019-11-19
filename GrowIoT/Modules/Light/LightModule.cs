using System;
using System.Collections.Generic;
using System.Device.Gpio;
using GrowIoT.Enums;
using GrowIoT.Interfaces;

namespace GrowIoT.Modules.Light
{
    public class LightModule : BaseModule
    {
        public LightModule(string name, List<ModuleRule> rules = null) : base(rules, name)
        {
            Type = ModuleType.Light;
        }
    }
}
