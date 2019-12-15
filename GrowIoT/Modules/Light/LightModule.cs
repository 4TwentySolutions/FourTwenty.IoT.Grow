using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Dto;
using FourTwenty.IoT.Connect.Models;

namespace GrowIoT.Modules.Light
{
    public class LightModule : BaseModule
    {
        public LightModule(string name, List<ModuleRuleDto> rules = null) : base(rules, name)
        {
            Type = ModuleType.Light;
        }
    }
}
