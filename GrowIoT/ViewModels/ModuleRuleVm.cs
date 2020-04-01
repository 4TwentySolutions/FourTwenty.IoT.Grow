using System;
using System.Collections.Generic;
using FourTwenty.IoT.Connect.Constants;
using FourTwenty.IoT.Connect.Entities;

namespace GrowIoT.ViewModels
{
    public class ModuleRuleVm : EntityViewModel<ModuleRule>
    {
        public int Id { get; set; }
        public JobType Job { get; set; }
        public RuleType RuleType { get; set; }
        public string RuleContent { get; set; }
        public int GrowBoxModuleId { get; set; }
        public List<int> Pins { get; set; }
        public int? Pin { get; set; }
        public bool IsEnabled { get; set; }
    }
}
