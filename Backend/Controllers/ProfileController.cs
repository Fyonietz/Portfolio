using Backend.Models;
using Backend.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public static class ProfileController
    {
        public static void MapProfile(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/profile");

            group.RequireAuthorization();

            group.MapGet("/", async (Database db) =>
            {
                using var conn = db.connect();
                var sql = @"SELECT Id, Name, Role_Title, Photo_Url, Status, Bio, Updated_At FROM Profile LIMIT 1";
                var profile = await conn.QueryFirstOrDefaultAsync<Profile>(sql);

                if (profile == null) return Results.NotFound();
                return Results.Ok(profile);
            });

            group.MapPut("/", async ([FromBody] Profile input, Database db) =>
            {
                using var conn = db.connect();
                var existing = await conn.QueryFirstOrDefaultAsync<Profile>("SELECT Id FROM Profile LIMIT 1");
                if (existing == null) return Results.NotFound();

                var now = DateTime.UtcNow;
                var sql = @"UPDATE Profile SET Name=@Name, Role_Title=@Role_Title, Photo_Url=@Photo_Url, Status=@Status, Bio=@Bio, Updated_At=@Updated_At WHERE Id=@Id";
                var parameters = new
                {
                    Name = input.Name,
                    Role_Title = input.Role_Title,
                    Photo_Url = input.Photo_Url,
                    Status = input.Status,
                    Bio = input.Bio,
                    Updated_At = now,
                    Id = existing.Id
                };

                var updated = await conn.ExecuteAsync(sql, parameters);
                if (updated == 0) return Results.NotFound();

                input.Id = existing.Id;
                input.Updated_At = now;
                return Results.Ok(input);
            });
        }
    }
}
