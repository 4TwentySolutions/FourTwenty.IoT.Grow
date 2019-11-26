using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Modules;

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
