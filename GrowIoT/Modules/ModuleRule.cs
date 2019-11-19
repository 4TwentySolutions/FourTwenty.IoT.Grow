using GrowIoT.Enums;

namespace GrowIoT.Modules
{
    public sealed class ModuleRule
    {
        public string ModuleName { get; set; }
        public string CronExpression { get; set; }
        public JobType Type { get; set; }
    }
}
