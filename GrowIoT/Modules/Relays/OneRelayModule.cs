using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;

namespace GrowIoT.Modules.Relays
{
    public class OneRelayModule : BaseModule
    {
        public OneRelayModule(int gpioPin, List<ModuleRuleDto> rules) : base(gpioPin, rules)
        {
            Type = ModuleType.Relay;
        }
    }
}
