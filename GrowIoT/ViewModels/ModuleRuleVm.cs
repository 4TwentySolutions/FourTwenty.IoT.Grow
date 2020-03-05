using System;
using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;

namespace GrowIoT.ViewModels
{
    public class ModuleRuleVm
    {
        public int Id { get; set; }
        public JobType Job { get; set; }
        public RuleType RuleType { get; set; }
        public string RuleContent { get; set; }
        public int GrowBoxModuleId { get; set; }
        public List<int> Pins { get; set; }
        public int? Pin { get; set; }
    }
}
