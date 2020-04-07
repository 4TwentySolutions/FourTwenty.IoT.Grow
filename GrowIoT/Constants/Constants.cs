using System;
using System.IO;

namespace GrowIoT.Constants
{
    public static class Constants
    {
        

        public static string ConfigFileName = "config.txt";
        public static string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);

        public static string LogsDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"{nameof(GrowIoT)}-logs");
    }
}
