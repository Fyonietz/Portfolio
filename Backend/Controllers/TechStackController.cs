
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers
{
    public static class TechStackController
    {
        public static void MapTechStack(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/tech-stack")
                       .DisableAntiforgery();

            // GET ALL
            g.MapGet("/", async (TechStackServices svc) =>
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
            g.MapGet("/{id}", async (TechStackServices svc, int id) =>
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
                TechStackServices svc,
                HttpRequest request) =>
            {
                try
                {
                    var form = await request.ReadFormAsync();

                    var icon = form.Files.Count > 0
                        ? await svc.SaveIcon(form.Files[0])
                        : null;

                    var techStack = new TechStack
                    {
                        Name = form["Name"].ToString(),
                        Icon_Url = icon,
                        Sort_Order = int.TryParse(
                            form["Sort_Order"],
                            out var sortOrder)
                                ? sortOrder
                                : 0
                    };

                    var result = await svc.Create(techStack);

                    return result
                        ? Results.Created(
                            $"/api/v1/tech-stack/{techStack.Id}",
                            techStack)
                        : Results.Problem(
                            "Failed to create tech stack.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // PUT
            g.MapPut("/{id}", async (
                TechStackServices svc,
                HttpRequest request,
                int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);

                    if (existing is null)
                        return Results.NotFound();

                    var form = await request.ReadFormAsync();

                    var icon = form.Files.Count > 0
                        ? await svc.SaveIcon(form.Files[0])
                        : existing.Icon_Url;

                    var techStack = new TechStack
                    {
                        Id = id,
                        Name = form["Name"].ToString(),
                        Icon_Url = icon,
                        Sort_Order = int.TryParse(
                            form["Sort_Order"],
                            out var sortOrder)
                                ? sortOrder
                                : existing.Sort_Order
                    };

                    var result = await svc.Update(id, techStack);

                    if (!result)
                        return Results.Problem(
                            "Failed to update tech stack.");

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
                TechStackServices svc,
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
                            "Failed to delete tech stack.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
