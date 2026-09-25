using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class ProfileServices
    {
        private readonly Database db;
        private readonly string uploadPath;

        public ProfileServices(Database _db, IConfiguration config)
        {
            db = _db;

            uploadPath = config["UploadPath"]
                ?? "wwwroot/uploads/profile";

            Directory.CreateDirectory(uploadPath);
        }

        // SAVE PHOTO
        public async Task<string> SavePhoto(IFormFile file)
        {
            if (file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadPath, fileName);

            using var stream = File.Create(fullPath);

            await file.CopyToAsync(stream);

            return $"/uploads/profile/{fileName}";
        }

        // GET ALL
        public async Task<IEnumerable<Profile>> GetAll()
        {
            using var conn = db.connect();

            return await conn.QueryAsync<Profile>(
                @"SELECT
                    Id,
                    Name,
                    Role_Title,
                    Description,
                    Photo_Url,
                    Status,
                    Bio,
                    Updated_At
                  FROM Profile
                  ORDER BY Id ASC"
            );
        }

        // GET BY ID
        public async Task<Profile> GetById(int id)
        {
            using var conn = db.connect();

            return await conn.QueryFirstOrDefaultAsync<Profile>(
                @"SELECT
                    Id,
                    Name,
                    Role_Title,
                    Description,
                    Photo_Url,
                    Status,
                    Bio,
                    Updated_At
                  FROM Profile
                  WHERE Id = @Id",
                new { Id = id }
            );
        }

        // CREATE
        public async Task<bool> Create(Profile data)
        {
            using var conn = db.connect();

            var now = DateTime.UtcNow;

            var sql = @"INSERT INTO Profile
                (
                    Name,
                    Role_Title,
                    Description,
                    Photo_Url,
                    Status,
                    Bio,
                    Updated_At
                )
                VALUES
                (
                    @Name,
                    @Role_Title,
                    @Description,
                    @Photo_Url,
                    @Status,
                    @Bio,
                    @Updated_At
                )";

            var res = await conn.ExecuteAsync(sql, new
            {
                Name = data.Name,
                Role_Title = data.Role_Title,
                Description = data.Description,
                Photo_Url = data.Photo_Url,
                Status = data.Status,
                Bio = data.Bio,
                Updated_At = now
            });

            if (res > 0)
            {
                data.Id = await conn.ExecuteScalarAsync<int>(
                    "SELECT last_insert_rowid()"
                );

                data.Updated_At = now;
            }

            return res > 0;
        }

        // UPDATE
        public async Task<bool> Update(int id, Profile data)
        {
            using var conn = db.connect();

            var now = DateTime.UtcNow;

            var sql = @"UPDATE Profile SET
                Name        = @Name,
                Role_Title  = @Role_Title,
                Description = @Description,
                Photo_Url   = @Photo_Url,
                Status      = @Status,
                Bio         = @Bio,
                Updated_At  = @Updated_At
                WHERE Id    = @Id";

            var res = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                Name = data.Name,
                Role_Title = data.Role_Title,
                Description = data.Description,
                Photo_Url = data.Photo_Url,
                Status = data.Status,
                Bio = data.Bio,
                Updated_At = now
            });

            return res > 0;
        }

        // DELETE
        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();

            var res = await conn.ExecuteAsync(
                "DELETE FROM Profile WHERE Id = @Id",
                new { Id = id }
            );

            return res > 0;
        }
    }
}
