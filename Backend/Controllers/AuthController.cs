using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
namespace Backend.Controllers
{
    public static class AuthController
    {
        public static void MapAuth(this WebApplication app)
        {
            var group = app.MapGroup("/api/v1/auth");

            group.MapPost("/register", async (
                [FromForm] RegisterRequest request,
                AuthServices services,
                IPasswordService pService,
                IJWTService jwtService) =>
            {
                if(await services.Registered()){
                  return Results.BadRequest("Registration Is Disabled.");
                }
                // Upload image
                string imageUrl = "";

                if (request.Image != null && request.Image.Length > 0)
                {
                    var uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads");

                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await request.Image.CopyToAsync(stream);

                    imageUrl = fileName;
                }

                // Hash password
                var hashedPassword = pService.HashPassword(request.Password);

                // Create user
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = hashedPassword,
                    ImageUrl = imageUrl
                };

                // Save to database
                var result = await services.Register(user);

                if (result == false)
                {
                    return Results.BadRequest("Register failed.");
                }

                // Generate JWT
                var token = jwtService.GenerateToken(user);

                return Results.Ok();
            }).DisableAntiforgery();
        }
    }
}
