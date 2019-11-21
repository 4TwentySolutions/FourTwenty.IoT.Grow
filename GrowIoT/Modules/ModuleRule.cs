using GrowIoT.Enums;

namespace GrowIoT.Modules
{
    public class ModuleRule
    {
        public string ModuleName { get; set; }
        public string CronExpression { get; set; }
        public JobType Type { get; set; }
    }

    public class PeriodRule : ModuleRule
    {
        /// <summary>
        /// Period in seconds
        /// </summary>
        public int Period { get; set; }
    }
}
