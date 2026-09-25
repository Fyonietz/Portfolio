
using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class TechStackServices
    {
        private readonly Database db;
        private readonly string uploadPath;

        public TechStackServices(
            Database _db,
            IConfiguration config)
        {
            db = _db;

            uploadPath = config["StackPath"]
                ?? "wwwroot/uploads/tech-stack";

            Directory.CreateDirectory(uploadPath);
        }

        // SAVE ICON
        public async Task<string> SaveIcon(IFormFile file)
        {
            if (file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using var stream = File.Create(fullPath);

            await file.CopyToAsync(stream);

            return $"/uploads/tech-stack/{fileName}";
        }

        // GET ALL
        public async Task<IEnumerable<TechStack>> GetAll()
        {
            using var conn = db.connect();

            return await conn.QueryAsync<TechStack>(
                @"SELECT
                    Id,
                    Name,
                    Icon_Url,
                    Sort_Order
                  FROM TechStack
                  ORDER BY Sort_Order ASC, Id ASC"
            );
        }

        // GET BY ID
        public async Task<TechStack> GetById(int id)
        {
            using var conn = db.connect();

            return await conn.QueryFirstOrDefaultAsync<TechStack>(
                @"SELECT
                    Id,
                    Name,
                    Icon_Url,
                    Sort_Order 
                  FROM TechStack
                  WHERE Id = @Id",
                new { Id = id }
            );
        }

        // CREATE
        public async Task<bool> Create(TechStack data)
        {
            using var conn = db.connect();

            var now = DateTime.UtcNow;

            var sql = @"INSERT INTO TechStack
                (
                    Name,
                    Icon_Url,
                    Sort_Order
                )
                VALUES
                (
                    @Name,
                    @Icon_Url,
                    @Sort_Order
                )";

            var res = await conn.ExecuteAsync(sql, new
            {
                Name = data.Name,
                Icon_Url = data.Icon_Url,
                Sort_Order = data.Sort_Order,
            });

            if (res > 0)
            {
                data.Id = await conn.ExecuteScalarAsync<int>(
                    "SELECT last_insert_rowid()"
                );

            }

            return res > 0;
        }

        // UPDATE
        public async Task<bool> Update(
            int id,
            TechStack data)
        {
            using var conn = db.connect();

            var now = DateTime.UtcNow;

            var sql = @"UPDATE TechStack SET
                Name       = @Name,
                Icon_Url   = @Icon_Url,
                Sort_Order = @Sort_Order 
                WHERE Id   = @Id";

            var res = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                Name = data.Name,
                Icon_Url = data.Icon_Url,
                Sort_Order = data.Sort_Order,
                Updated_At = now
            });

            return res > 0;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();

            var res = await conn.ExecuteAsync(
                "DELETE FROM TechStack WHERE Id = @Id",
                new { Id = id }
            );

            return res > 0;
        }
    }
}
