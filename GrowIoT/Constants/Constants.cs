using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GrowIoT.Constants
{
    public static class Constants
    {
        public static string ConfigFileName = "config.txt";
        public static string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
    }
}
