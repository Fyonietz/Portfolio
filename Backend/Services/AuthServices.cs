using Backend.Models;
using Dapper;
namespace Backend.Services{
  public class AuthServices{
    private readonly Database db;
    public AuthServices(Database _db)=>db=_db;
    
    public async Task<bool> Register(User user){
      using var conn = db.connect();
      string sql  = @"INSERT INTO User(Username,Email,Password,ImageUrl) VALUES(@Name,@Password,@Email,@ImageUrl)";
      return await conn.ExecuteAsync(sql,user) > 0;
    }
    public async Task<bool> Registered(){
      using var conn = db.connect();
      var count =await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM User");
      return count > 0;
    }
  }
}
