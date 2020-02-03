using Microsoft.Data.Sqlite;

namespace Infrastructure.Data
{
    public interface ISqLiteProvider
    {
        SqliteConnection GetConnection(bool forceNewConnection = false);
    }
}
