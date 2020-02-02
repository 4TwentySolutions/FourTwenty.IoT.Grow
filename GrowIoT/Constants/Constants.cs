using System.IO;

namespace GrowIoT.Constants
{
    public static class Constants
    {
        public static string ConfigFileName = "config.txt";
        public static string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
    }
}
