using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;

namespace GrowIoT.Modules.Fans
{
    public class FanModule : BaseModule
    {
        public FanModule(string name, int? gpioPin = null, List<ModuleRuleDto> rules = null) : base(gpioPin, rules, name)
        {
            Type = ModuleType.Fan;
        }
    }
}
