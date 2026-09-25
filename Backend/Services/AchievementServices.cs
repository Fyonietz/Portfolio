using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class AchievementServices
    {
        private readonly Database db;
        private readonly string uploadPath;

        public AchievementServices(
            Database _db,
            IConfiguration config)
        {
            db = _db;

            uploadPath = config["AchievementPath"]
                ?? "wwwroot/uploads/achievements";

            Directory.CreateDirectory(uploadPath);
        }

        // SAVE PHOTO
        public async Task<string> SavePhoto(
            IFormFile file,
            string slug)
        {
            if (file.Length == 0)
                return null;

            var safeSlug = string.IsNullOrWhiteSpace(slug)
                ? "achievement"
                : slug.Trim();

            var achievementPath = Path.Combine(
                uploadPath,
                safeSlug
            );

            Directory.CreateDirectory(achievementPath);

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";

            var fullPath = Path.Combine(
                achievementPath,
                fileName
            );

            using var stream = File.Create(fullPath);

            await file.CopyToAsync(stream);

            return $"/uploads/achievements/{safeSlug}/{fileName}";
        }

        // GET ALL
        public async Task<IEnumerable<Achievement>> GetAll()
        {
            using var conn = db.connect();

            return await conn.QueryAsync<Achievement>(
                @"SELECT
                    Id,
                    Title,
                    Slug,
                    Description,
                    Photo_Url,
                    Achieved_At,
                    Sort_Order
                  FROM Achievement
                  ORDER BY Sort_Order ASC, Id ASC"
            );
        }

        // GET BY ID
        public async Task<Achievement> GetById(int id)
        {
            using var conn = db.connect();

            return await conn.QueryFirstOrDefaultAsync<Achievement>(
                @"SELECT
                    Id,
                    Title,
                    Slug,
                    Description,
                    Photo_Url,
                    Achieved_At,
                    Sort_Order
                  FROM Achievement
                  WHERE Id = @Id",
                new { Id = id }
            );
        }

        // CREATE
        public async Task<bool> Create(Achievement data)
        {
            using var conn = db.connect();

            var sql = @"INSERT INTO Achievement
                (
                    Title,
                    Slug,
                    Description,
                    Photo_Url,
                    Achieved_At,
                    Sort_Order
                )
                VALUES
                (
                    @Title,
                    @Slug,
                    @Description,
                    @Photo_Url,
                    @Achieved_At,
                    @Sort_Order
                )";

            var res = await conn.ExecuteAsync(sql, new
            {
                Title = data.Title,
                Slug = data.Slug,
                Description = data.Description,
                Photo_Url = data.Photo_Url,
                Achieved_At = data.Achieved_At,
                Sort_Order = data.Sort_Order
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
            Achievement data)
        {
            using var conn = db.connect();

            var sql = @"UPDATE Achievement SET
                Title        = @Title,
                Slug         = @Slug,
                Description  = @Description,
                Photo_Url    = @Photo_Url,
                Achieved_At  = @Achieved_At,
                Sort_Order   = @Sort_Order
                WHERE Id     = @Id";

            var res = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                Title = data.Title,
                Slug = data.Slug,
                Description = data.Description,
                Photo_Url = data.Photo_Url,
                Achieved_At = data.Achieved_At,
                Sort_Order = data.Sort_Order
            });

            return res > 0;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();

            var res = await conn.ExecuteAsync(
                "DELETE FROM Achievement WHERE Id = @Id",
                new { Id = id }
            );

            return res > 0;
        }
    }
}
