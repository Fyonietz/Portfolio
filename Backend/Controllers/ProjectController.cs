using Backend.Services;
using Backend.Models;

namespace Backend.Controllers
{
    public static class ProjectController
    {
        public static void MapProject(this WebApplication app)
        {
            var g = app.MapGroup("api/v1/project").DisableAntiforgery();

            // GET all
            g.MapGet("/", async (ProjectServices svc) =>
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
            g.MapGet("/id/{id}", async (ProjectServices svc, int id) =>
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

            // GET by slug
            g.MapGet("/{slug}", async (ProjectServices svc, string slug) =>
            {
                try
                {
                    var data = await svc.GetBySlug(slug);
                    return data is null ? Results.NotFound() : Results.Ok(data);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // POST
            g.MapPost("/", async (ProjectServices svc, HttpRequest request) =>
            {
                try
                {
                    var form   = await request.ReadFormAsync();
                    var photos = await svc.SavePhotos(form.Files);

                    // parse TechStack_Ids dari form: "1,2,3"
                    var techIds = form["TechStack_Ids"].ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x.Trim(), out var n) ? n : 0)
                        .Where(x => x > 0)
                        .ToList();

                    var project = new ProjectRequest
                    {
                        Title             = form["Title"].ToString(),
                        Slug              = form["Slug"].ToString(),
                        Short_Description = form["Short_Description"].ToString(),
                        Full_Description  = form["Full_Description"].ToString(),
                        Photos            = photos,
                        Repo_Url          = form["Repo_Url"].ToString(),
                        Demo_Url          = form["Demo_Url"].ToString(),
                        Sort_Order        = int.TryParse(form["Sort_Order"], out var s) ? s : 0,
                        Published         = bool.TryParse(form["Published"], out var p) && p,
                        TechStack_Ids     = techIds,
                    };

                    var result = await svc.Create(project);
                    return result
                        ? Results.Created($"/api/v1/project/{project.Slug}", project)
                        : Results.Problem("Failed to create project.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // PUT
            g.MapPut("/{id}", async (ProjectServices svc, HttpRequest request, int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();

                    var form   = await request.ReadFormAsync();
                    var photos = form.Files.Count > 0
                        ? await svc.SavePhotos(form.Files)
                        : existing.Photos;

                    var techIds = form["TechStack_Ids"].ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => int.TryParse(x.Trim(), out var n) ? n : 0)
                        .Where(x => x > 0)
                        .ToList();

                    var project = new ProjectRequest
                    {
                        Title             = form["Title"].ToString(),
                        Slug              = form["Slug"].ToString(),
                        Short_Description = form["Short_Description"].ToString(),
                        Full_Description  = form["Full_Description"].ToString(),
                        Photos            = photos,
                        Repo_Url          = form["Repo_Url"].ToString(),
                        Demo_Url          = form["Demo_Url"].ToString(),
                        Sort_Order        = int.TryParse(form["Sort_Order"], out var s) ? s : 0,
                        Published         = bool.TryParse(form["Published"], out var p) && p,
                        TechStack_Ids     = techIds,
                    };

                    var result = await svc.Update(id, project);
                    return result
                        ? Results.Ok(project)
                        : Results.Problem("Failed to update project.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);

            // DELETE
            g.MapDelete("/{id}", async (ProjectServices svc, int id) =>
            {
                try
                {
                    var existing = await svc.GetById(id);
                    if (existing is null) return Results.NotFound();

                    var result = await svc.Delete(id);
                    return result
                        ? Results.NoContent()
                        : Results.Problem("Failed to delete project.");
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            }).RequireAuthorization(Policies.Admin);
        }
    }
}
