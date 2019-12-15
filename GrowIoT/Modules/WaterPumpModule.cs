using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;

namespace GrowIoT.Modules
{
    public class WaterPumpModule : BaseModule
    {
        public WaterPumpModule(int? gpioPin = null, List<ModuleRuleDto> rules = null, string name = null) : base(gpioPin, rules, name)
        {
            Type = ModuleType.WaterPump;
        }
    }
}
