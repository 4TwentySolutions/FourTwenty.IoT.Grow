using FourTwenty.IoT.Connect.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrowIoT.ViewModels
{
    public class ModuleVm
    {
        public int Id{get;set;}
        [Required]
        public string Name { get; set; }
        public ModuleType Type { get; set; }
        public int[] Pins { get; set; }
        public int GrowBoxId { get; set; }
        public GrowBoxViewModel GrowBox { get; set; }
        public ICollection<ModuleRuleVm> Rules { get; set; }
    }
}
