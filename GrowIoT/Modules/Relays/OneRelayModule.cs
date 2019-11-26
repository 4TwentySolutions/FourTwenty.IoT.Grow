using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Modules;

namespace GrowIoT.Modules.Relays
{
    public class OneRelayModule : BaseModule
    {
        public OneRelayModule(int gpioPin, List<ModuleRule> rules) : base(gpioPin, rules)
        {
            Type = ModuleType.Relay;
        }
    }
}
