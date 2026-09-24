using Dapper;
using Backend.Models;

namespace Backend.Services
{
    public class ProjectServices
    {
        private readonly Database db;
        private readonly string uploadPath;

        public ProjectServices(Database _db, IConfiguration config)
        {
            db = _db;
            uploadPath = config["UploadPath"] ?? "wwwroot/uploads/projects";
            Directory.CreateDirectory(uploadPath);
        }

        public async Task<string> SavePhotos(IFormFileCollection files)
        {
            var paths = new List<string>();

            foreach (var file in files)
            {
                if (file.Length == 0) continue;
                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var fullPath = Path.Combine(uploadPath, fileName);
                using var stream = File.Create(fullPath);
                await file.CopyToAsync(stream);
                paths.Add($"/uploads/projects/{fileName}");
            }

            return string.Join(",", paths);
        }

        public async Task<bool> Create(Project data)
        {
            using var conn = db.connect();
            string sql = "INSERT INTO Project(Title,Slug,Short_Description,Full_Description,Photos,Tech_Stack,Repo_Url,Demo_Url,Sort_Order,Published) VALUES(@Title,@Slug,@SDesc,@FDesc,@Photos,@Tech,@RUrl,@DUrl,@SOrder,@Published)";
            var res = await conn.ExecuteAsync(sql, new
            {
                Title = data.Title,
                Slug = data.Slug,
                SDesc = data.Short_Description,
                FDesc = data.Full_Description,
                Photos = data.Photos,
                Tech = data.Tech_Stack,
                RUrl = data.Repo_Url,
                DUrl = data.Demo_Url,
                SOrder = data.Sort_Order,
                Published = data.Published
            });
            return res > 0;
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            using var conn = db.connect();
            return await conn.QueryAsync<Project>(
                "SELECT * FROM Project ORDER BY Sort_Order ASC"
            );
        }

        public async Task<Project> GetById(int id)
        {
            using var conn = db.connect();
            return await conn.QueryFirstOrDefaultAsync<Project>(
                "SELECT * FROM Project WHERE Id = @Id", new { Id = id }
            );
        }
        public async Task<Project> GetBySlug(string slug)
        {
            using var conn = db.connect();
            return await conn.QueryFirstOrDefaultAsync<Project>(
                "SELECT * FROM Project WHERE Slug = @Slug", new { Slug = slug }
            );
        }
        public async Task<bool> Update(int id, Project data)
        {
            using var conn = db.connect();
            string sql = @"UPDATE Project SET
                Title             = @Title,
                Slug              = @Slug,
                Short_Description = @SDesc,
                Full_Description  = @FDesc,
                Photos            = @Photos,
                Tech_Stack        = @Tech,
                Repo_Url          = @RUrl,
                Demo_Url          = @DUrl,
                Sort_Order        = @SOrder,
                Published         = @Published
                WHERE Id = @Id";
            var res = await conn.ExecuteAsync(sql, new
            {
                Id = id,
                Title = data.Title,
                Slug = data.Slug,
                SDesc = data.Short_Description,
                FDesc = data.Full_Description,
                Photos = data.Photos,
                Tech = data.Tech_Stack,
                RUrl = data.Repo_Url,
                DUrl = data.Demo_Url,
                SOrder = data.Sort_Order,
                Published = data.Published
            });
            return res > 0;
        }

        public async Task<bool> Delete(int id)
        {
            using var conn = db.connect();
            var res = await conn.ExecuteAsync(
                "DELETE FROM Project WHERE Id = @Id", new { Id = id }
            );
            return res > 0;
        }

    }//Class
}//Namespace
