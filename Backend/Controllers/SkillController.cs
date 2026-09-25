using Backend.Services;
using Backend.Models;

namespace Backend.Controllers
{
    public static class SkillController
    {
        public static void MapSkill(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/skill").DisableAntiforgery();

            // GET all
            g.MapGet("/", async (SkillServices svc) =>
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

            // GET by id
            g.MapGet("/{id}", async (SkillServices svc, int id) =>
            {
                try
                {
                    var data = await svc.GetById(id);
                    return data is null ? Results.NotFound() : Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // POST
            g.MapPost("/", async (SkillServices svc, Skill data) =>
            {
                try
                {
                    var result = await svc.Create(data);
                    return result
                        ? Results.Created($"/api/v1/skill", data)
                        : Results.Problem("Failed to create skill.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // PUT
            g.MapPut("/{id}", async (SkillServices svc, int id, Skill data) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();

                    var result = await svc.Update(id, data);
                    return result
                        ? Results.Ok(data)
                        : Results.Problem("Failed to update skill.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // DELETE
            g.MapDelete("/{id}", async (SkillServices svc, int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();

                    var result = await svc.Delete(id);
                    return result
                        ? Results.NoContent()
                        : Results.Problem("Failed to delete skill.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
