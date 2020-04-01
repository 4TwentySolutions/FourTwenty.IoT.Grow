using System;
using FourTwenty.Core.Data.Interfaces;
using Microsoft.Data.Sqlite;

namespace Infrastructure.Data
{
    public class SqLiteProvider : ISqlProvider<SqliteConnection>, IDisposable
    {
        private bool _disposed;
        private SqliteConnection _asyncConnection;

        public SqliteConnection GetConnection(bool forceNewConnection = false)
        {
            SqliteConnection connection = _asyncConnection;
            if (forceNewConnection || _asyncConnection == null)
                connection = _asyncConnection = CreateConnection();
            return connection;
        }

        private GrowSqlConnectionAsync CreateConnection()
        {
            var connection = new GrowSqlConnectionAsync();
#if NETSTANDARD2_1
         connection.ConnectionString = new SqliteConnectionStringBuilder(connection.ConnectionString)
            { Password = Constants.SqlPassword }
                .ToString();
            connection.Open();
#elif NETSTANDARD2_0
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT quote($password);";
            command.Parameters.AddWithValue("$password", Constants.SqlPassword);
            var quotedPassword = (string)command.ExecuteScalar();
            command.CommandText = "PRAGMA key = " + quotedPassword;
            command.Parameters.Clear();
            command.ExecuteNonQuery();
#endif
            return connection;
        }

        #region dispose pattern
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (!disposing) return;

            try
            {
                if (_asyncConnection == null)
                    return;
                _asyncConnection.Close();
                _asyncConnection.Dispose();
                _asyncConnection = null;
            }
            finally
            {
                _disposed = true;
            }
        }
        #endregion
    }
}
