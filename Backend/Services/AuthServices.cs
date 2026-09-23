using Backend.Models;
using Dapper;
namespace Backend.Services
{
    public class AuthServices
    {
        private readonly Database db;
        private readonly IPasswordService pService;

        public AuthServices(Database _db, IPasswordService _p)
        {
            db = _db;
            pService = _p;
        }

        public async Task<bool> Register(User user)
        {
            using var conn = db.connect();
            string sql = @"INSERT INTO User(Username,Email,Password,ImageUrl) VALUES(@Name,@Email,@Password,@ImageUrl); SELECT last_insert_rowid();";
            var id = await conn.ExecuteScalarAsync<long>(sql, user);
            if (id > 0)
            {
                user.Id = (int)id;
                return true;
            }
            return false;
        }

        public async Task<bool> Registered()
        {
            using var conn = db.connect();
            var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM User");
            return count > 0;
        }

        public async Task<User> Login(string email, string password)
        {
            using var conn = db.connect();
            var sql = @"SELECT Id, Username, Password, Email, ImageUrl FROM User WHERE Email=@Email LIMIT 1";
            var user = await conn.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
            if (user == null) return null;

            if (!pService.VerifyPassword(password, user.Password)) return null;

            // clear password before returning
            user.Password = string.Empty;
            return user;
        }
    }
}
