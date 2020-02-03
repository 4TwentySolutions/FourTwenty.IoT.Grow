using Microsoft.Data.Sqlite;

namespace Infrastructure.Data
{
    public class GrowSqlConnectionAsync : SqliteConnection
    {
        public GrowSqlConnectionAsync() : base($"Filename={Constants.DatabasePath}")
        {
        }

    }
}
