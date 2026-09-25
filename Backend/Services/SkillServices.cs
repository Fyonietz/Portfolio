using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class SkillServices
    {
        private readonly Database db;

        public SkillServices(Database _db)
        {
            db = _db;
        }

        public async Task<IEnumerable<Skill>> GetAll()
        {
            using var conn = db.connect();
            return await conn.QueryAsync<Skill>(
                "SELECT * FROM Skill ORDER BY Sort_Order ASC"
            );
        }

        public async Task<Skill?> GetById(int id)
        {
            using var conn = db.connect();
            return await conn.QueryFirstOrDefaultAsync<Skill>(
                "SELECT * FROM Skill WHERE Id = @Id", new { Id = id }
            );
        }

        public async Task<bool> Create(Skill data)
        {
            using var conn = db.connect();
            string sql = @"
                INSERT INTO Skill
                    (Name, Category, Description, Icon_Url, Level,
                     Sort_Order, Published, Created_At, Updated_At)
                VALUES
                    (@Name, @Category, @Description, @Icon_Url, @Level,
                     @Sort_Order, @Published, datetime('now'), datetime('now'))";
            var res = await conn.ExecuteAsync(sql, new {
                Name        = data.Name,
                Category    = data.Category,
                Description = data.Description,
                Icon_Url    = data.Icon_Url,
                Level       = data.Level,
                Sort_Order  = data.Sort_Order,
                Published   = data.Published,
            });
            return res > 0;
        }

        public async Task<bool> Update(int id, Skill data)
        {
            using var conn = db.connect();
            string sql = @"
                UPDATE Skill SET
                    Name        = @Name,
                    Category    = @Category,
                    Description = @Description,
                    Icon_Url    = @Icon_Url,
                    Level       = @Level,
                    Sort_Order  = @SOrder,
                    Published   = @Published,
                    Updated_At  = datetime('now')
                WHERE Id = @Id";
            var res = await conn.ExecuteAsync(sql, new {
                Id          = id,
                Name        = data.Name,
                Category    = data.Category,
                Description = data.Description,
                Icon_Url    = data.Icon_Url,
                Level       = data.Level,
                SOrder      = data.Sort_Order,
                Published   = data.Published,
            });
            return res > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                "DELETE FROM Skill WHERE Id = @Id", new { Id = id }
            );
            return res > 0;
        }

    }//Class
}//Namespace
