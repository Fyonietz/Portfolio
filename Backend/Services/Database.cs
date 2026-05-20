using Microsoft.Data.Sqlite;

namespace Portfolio.Services{
    public class Database{
      private readonly string _connectionString = "Data Source=app.db";
      
      public SqliteConnection connect(){
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
      }
    }
}
