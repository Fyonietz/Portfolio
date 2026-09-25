using Backend.Services;
using Backend.Models;

namespace Backend.Controllers
{
    public static class AchievementController
    {
        public static void MapAchievement(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/achievement").DisableAntiforgery();

            g.MapGet("/", async (AchievementServices svc) =>
            {
                try
                {
                    return Results.Ok(await svc.GetAll());
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            });

            g.MapGet("/{id}", async (AchievementServices svc, int id) =>
            {
                try
                {
                    var data = await svc.GetById(id);
                    return data is null ? Results.NotFound() : Results.Ok(data);
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            });

            g.MapPost("/", async (AchievementServices svc, Achievement data) =>
            {
                try
                {
                    var result = await svc.Create(data);
                    return result
                        ? Results.Created("/api/v1/achievement", data)
                        : Results.Problem("Failed to create.");
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            }).RequireAuthorization(Policies.Admin);

            g.MapPut("/{id}", async (AchievementServices svc, int id, Achievement data) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();
                    var result = await svc.Update(id, data);
                    return result
                        ? Results.Ok(data)
                        : Results.Problem("Failed to update.");
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            }).RequireAuthorization(Policies.Admin);

            g.MapDelete("/{id}", async (AchievementServices svc, int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();
                    var result = await svc.Delete(id);
                    return result
                        ? Results.NoContent()
                        : Results.Problem("Failed to delete.");
                }
                catch (Exception ex) { return Results.Problem(ex.Message); }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
