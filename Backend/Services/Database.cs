using Microsoft.Data.Sqlite;

namespace Backend.Services
{
    public class Database
    {
        private readonly string _connectionString = Env.Value["Database:connection"]!;

        public SqliteConnection connect()
        {
            var connection = new SqliteConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }//Class
}//Namespace
