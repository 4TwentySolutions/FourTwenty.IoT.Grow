using System.Collections.Generic;
using GrowIoT.Modules;

namespace GrowIoT.Models
{
    public class ConfigModel
    {
        public IList<BaseModule> Modules { get; set; }
        public int ListeningPort { get; set; }
    }


}
