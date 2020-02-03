using System.IO;

namespace Infrastructure
{
    public static class Constants
    {
        public static string SqlPassword = "123456Qq@";
        public static string DatabasePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "database.db");
    }
}
