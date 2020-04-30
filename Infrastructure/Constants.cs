using System.IO;

namespace Infrastructure
{
    public static class Constants
    {
        public const int BoxId = 1;
        public static string SqlPassword = "123456Qq@";
        public static string DatabasePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "database.db");
        public static string HistoryDatabasePath => Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "historyDatabase.db");
    }
}
