using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using GrowIoT.Enums;
using GrowIoT.Interfaces;

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
