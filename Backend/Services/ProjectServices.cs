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

        private async Task<List<TechStack>> GetTechStacks(int projectId)
        {
            using var conn = db.connect();
            var sql = @"
                SELECT t.Id, t.Name, t.Icon_Url, t.Sort_Order
                FROM TechStack t
                INNER JOIN Project_TechStack pt ON pt.TechStack_Id = t.Id
                WHERE pt.Project_Id = @ProjectId
                ORDER BY t.Sort_Order ASC";
            var result = await conn.QueryAsync<TechStack>(sql, new { ProjectId = projectId });
            return result.ToList();
        }

        private async Task InsertTechStacks(int projectId, List<int> techIds)
        {
            using var conn = db.connect();
            foreach (var techId in techIds)
            {
                await conn.ExecuteAsync(
                    "INSERT OR IGNORE INTO Project_TechStack(Project_Id, TechStack_Id) VALUES(@PId, @TId)",
                    new { PId = projectId, TId = techId }
                );
            }
        }

        public async Task<IEnumerable<Project>> GetAll()
        {
            using var conn = db.connect();
            var projects = (await conn.QueryAsync<Project>(
                "SELECT * FROM Project ORDER BY Sort_Order ASC"
            )).ToList();

            foreach (var p in projects)
                p.TechStacks = await GetTechStacks(p.Id);

            return projects;
        }

        public async Task<Project?> GetById(int id)
        {
            using var conn = db.connect();
            var project = await conn.QueryFirstOrDefaultAsync<Project>(
                "SELECT * FROM Project WHERE Id = @Id", new { Id = id }
            );
            if (project is null) return null;
            project.TechStacks = await GetTechStacks(project.Id);
            return project;
        }

        public async Task<Project?> GetBySlug(string slug)
        {
            using var conn = db.connect();
            var project = await conn.QueryFirstOrDefaultAsync<Project>(
                "SELECT * FROM Project WHERE Slug = @Slug", new { Slug = slug }
            );
            if (project is null) return null;
            project.TechStacks = await GetTechStacks(project.Id);
            return project;
        }

        public async Task<bool> Create(ProjectRequest data)
        {
            using var conn = db.connect();
            string sql = @"
                INSERT INTO Project
                    (Title, Slug, Short_Description, Full_Description, Photos,
                     Repo_Url, Demo_Url, Sort_Order, Published, Created_At, Updated_At)
                VALUES
                    (@Title, @Slug, @SDesc, @FDesc, @Photos,
                     @RUrl, @DUrl, @SOrder, @Published,
                     datetime('now'), datetime('now'));
                SELECT last_insert_rowid();";

            var id = await conn.ExecuteScalarAsync<long>(sql, new {
                Title     = data.Title,
                Slug      = data.Slug,
                SDesc     = data.Short_Description,
                FDesc     = data.Full_Description,
                Photos    = data.Photos,
                RUrl      = data.Repo_Url,
                DUrl      = data.Demo_Url,
                SOrder    = data.Sort_Order,
                Published = data.Published,
            });

            if (id <= 0) return false;

            if (data.TechStack_Ids.Any())
                await InsertTechStacks((int)id, data.TechStack_Ids);

            return true;
        }

        public async Task<bool> Update(int id, ProjectRequest data)
        {
            using var conn = db.connect();
            string sql = @"
                UPDATE Project SET
                    Title             = @Title,
                    Slug              = @Slug,
                    Short_Description = @SDesc,
                    Full_Description  = @FDesc,
                    Photos            = @Photos,
                    Repo_Url          = @RUrl,
                    Demo_Url          = @DUrl,
                    Sort_Order        = @SOrder,
                    Published         = @Published,
                    Updated_At        = datetime('now')
                WHERE Id = @Id";

            var res = await conn.ExecuteAsync(sql, new {
                Id        = id,
                Slug      = data.Slug,
                Title     = data.Title,
                SDesc     = data.Short_Description,
                FDesc     = data.Full_Description,
                Photos    = data.Photos,
                RUrl      = data.Repo_Url,
                DUrl      = data.Demo_Url,
                SOrder    = data.Sort_Order,
                Published = data.Published,
            });

            if (res <= 0) return false;

            using var conn2 = db.connect();
            await conn2.ExecuteAsync(
                "DELETE FROM Project_TechStack WHERE Project_Id = @Id", new { Id = id }
            );

            if (data.TechStack_Ids.Any())
                await InsertTechStacks(id, data.TechStack_Ids);

            return true;
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
