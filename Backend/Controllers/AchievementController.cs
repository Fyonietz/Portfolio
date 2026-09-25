using Backend.Services;
using Backend.Models;

namespace Backend.Controllers
{
    public static class AchievementController
    {
        public static void MapAchievement(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/achievement")
                       .DisableAntiforgery();

            // GET ALL
            g.MapGet("/", async (AchievementServices svc) =>
            {
                try
                {
                    var data = await svc.GetAll();
                    return Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // GET BY ID
            g.MapGet("/{id}", async (
                AchievementServices svc,
                int id) =>
            {
                try
                {
                    var data = await svc.GetById(id);

                    return data is null
                        ? Results.NotFound()
                        : Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // POST
            g.MapPost("/", async (
                AchievementServices svc,
                HttpRequest request) =>
            {
                try
                {
                    var form = await request.ReadFormAsync();

                    var title = form["Title"].ToString();
                    var slug = form["Slug"].ToString();

                    var photo = form.Files.Count > 0
                        ? await svc.SavePhoto(
                            form.Files[0],
                            slug)
                        : null;

                    var achievement = new Achievement
                    {
                        Title = title,
                        Slug = slug,
                        Description = form["Description"].ToString(),
                        Photo_Url = photo,
                        Achieved_At = form["Achieved_At"].ToString(),
                        Sort_Order = int.TryParse(
                            form["Sort_Order"],
                            out var sortOrder)
                                ? sortOrder
                                : 0
                    };

                    var result = await svc.Create(achievement);

                    return result
                        ? Results.Created(
                            $"/api/v1/achievement/{achievement.Id}",
                            achievement)
                        : Results.Problem(
                            "Failed to create achievement.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // PUT
            g.MapPut("/{id}", async (
                AchievementServices svc,
                HttpRequest request,
                int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);

                    if (existing is null)
                        return Results.NotFound();

                    var form = await request.ReadFormAsync();

                    var title = form["Title"].ToString();
                    var slug = form["Slug"].ToString();

                    // Keep old photo if no new photo uploaded
                    var photo = form.Files.Count > 0
                        ? await svc.SavePhoto(
                            form.Files[0],
                            slug)
                        : existing.Photo_Url;

                    var achievement = new Achievement
                    {
                        Id = id,
                        Title = title,
                        Slug = slug,
                        Description = form["Description"].ToString(),
                        Photo_Url = photo,
                        Achieved_At = form["Achieved_At"].ToString(),
                        Sort_Order = int.TryParse(
                            form["Sort_Order"],
                            out var sortOrder)
                                ? sortOrder
                                : existing.Sort_Order
                    };

                    var result = await svc.Update(
                        id,
                        achievement);

                    if (!result)
                        return Results.Problem(
                            "Failed to update achievement.");

                    var updated = await svc.GetById(id);

                    return Results.Ok(updated);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // DELETE
            g.MapDelete("/{id}", async (
                AchievementServices svc,
                int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);

                    if (existing is null)
                        return Results.NotFound();

                    var result = await svc.Delete(id);

                    return result
                        ? Results.NoContent()
                        : Results.Problem(
                            "Failed to delete achievement.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
