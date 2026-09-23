using Backend.Models;
using Backend.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    public static class ContactController
    {
        public static void MapContact(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/contact");

            group.RequireAuthorization();

            group.MapGet("/", async (Database db) =>
            {
                using var conn = db.connect();
                var sql = @"SELECT Id, Platform, Value, Sort_Order FROM Contact ORDER BY Sort_Order";
                var list = await conn.QueryAsync<Contact>(sql);
                return Results.Ok(list);
            });

            group.MapPost("/", async ([FromBody] Contact input, Database db) =>
            {
                using var conn = db.connect();
                var sql = @"INSERT INTO Contact(Platform, Value, Sort_Order) VALUES(@Platform, @Value, @Sort_Order); SELECT last_insert_rowid();";
                var id = await conn.ExecuteScalarAsync<long>(sql, input);
                input.Id = (int)id;
                return Results.Created($"/api/v1/contact/{input.Id}", input);
            });

            group.MapPut("/{id}", async (int id, [FromBody] Contact input, Database db) =>
            {
                using var conn = db.connect();
                var exists = await conn.QueryFirstOrDefaultAsync<Contact>("SELECT Id FROM Contact WHERE Id=@Id", new { Id = id });
                if (exists == null) return Results.NotFound();

                var sql = @"UPDATE Contact SET Platform=@Platform, Value=@Value, Sort_Order=@Sort_Order WHERE Id=@Id";
                var updated = await conn.ExecuteAsync(sql, new { Platform = input.Platform, Value = input.Value, Sort_Order = input.Sort_Order, Id = id });
                if (updated == 0) return Results.NotFound();

                input.Id = id;
                return Results.Ok(input);
            });

            group.MapDelete("/{id}", async (int id, Database db) =>
            {
                using var conn = db.connect();
                var deleted = await conn.ExecuteAsync("DELETE FROM Contact WHERE Id=@Id", new { Id = id });
                if (deleted == 0) return Results.NotFound();
                return Results.NoContent();
            });
        }
    }
}
