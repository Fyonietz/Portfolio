using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class AchievementServices
    {
        private readonly Database db;

        public AchievementServices(Database _db)
        {
            db = _db;
        }

        public async Task<IEnumerable<Achievement>> GetAll()
        {
            using var conn = db.connect();
            return await conn.QueryAsync<Achievement>(
                "SELECT * FROM Achievement ORDER BY Sort_Order ASC"
            );
        }

        public async Task<Achievement?> GetById(int id)
        {
            using var conn = db.connect();
            return await conn.QueryFirstOrDefaultAsync<Achievement>(
                "SELECT * FROM Achievement WHERE Id = @Id", new { Id = id }
            );
        }

        public async Task<bool> Create(Achievement data)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                @"INSERT INTO Achievement(Title, Description, Achieved_At, Sort_Order)
                  VALUES(@Title, @Description, @Achieved_At, @Sort_Order)",
                new {
                    Title       = data.Title,
                    Description = data.Description,
                    Achieved_At = data.Achieved_At,
                    Sort_Order  = data.Sort_Order,
                }
            );
            return res > 0;
        }

        public async Task<bool> Update(int id, Achievement data)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                @"UPDATE Achievement SET
                    Title       = @Title,
                    Description = @Description,
                    Achieved_At = @Achieved_At,
                    Sort_Order  = @SOrder
                  WHERE Id = @Id",
                new {
                    Id          = id,
                    Title       = data.Title,
                    Description = data.Description,
                    Achieved_At = data.Achieved_At,
                    SOrder      = data.Sort_Order,
                }
            );
            return res > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                "DELETE FROM Achievement WHERE Id = @Id", new { Id = id }
            );
            return res > 0;
        }

    }//Class
}//Namespace
