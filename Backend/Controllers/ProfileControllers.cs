using Portfolio.Services;
using Portfolio.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
namespace Portfolio.Controllers
{
    public static class ProfileControllers
    {
        public static void MapProfile(this WebApplication app)
        {

            var g = app.MapGroup("/api/v1/profile");


            g.MapPost("/", async (
                [FromForm] string name,
                [FromForm] string role,
                [FromForm] string status,
                [FromForm] string bio,
                IFormFile? image, 
                [FromServices] ProfileServices services) =>
            {
                try
                {
                    var profile = new Profile
                    {
                        Name = name,
                        Role_Title = role,
                        Status = status,
                        Bio = bio
                    };

                    string? imagePath = null;


                    if (image != null && image.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        imagePath = $"/uploads/{uniqueFileName}";
                    }


                    profile.Photo_Url = imagePath;
                    await services.Create(profile);

                    return Results.Ok(new { message = "Profile created successfully!", imagePath });
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }
            });

        g.MapGet("/",async (ProfileServices services)=>{
            try{
              var req = await services.GetAllProfile();
              return Results.Ok(req);
            }catch(Exception e){
              return Results.InternalServerError(e.Message);
            }
            });
        }
    }
}
