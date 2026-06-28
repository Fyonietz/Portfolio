using Portfolio.Models;
using Portfolio.Services;
using Microsoft.AspNetCore.Antiforgery;
namespace Portfolio.Controllers
{

    public static class AuthControllers
    {

        public static void MapAuth(this WebApplication app)
        {

            var g = app.MapGroup("/api/v1/auth");

            g.MapPost("/admin-creation", async (AuthServices services, IPasswordService p, Admin admin) =>
            {

                try
                {
                    Admin data = new Admin
                    {
                        Id = admin.Id,
                        Username = admin.Username,
                        Password = await p.HashPasswordAsync(admin.Password),
                        Email = admin.Email,
                        Created_At = admin.Created_At
                    };
                    var result = await services.Create(data);
                    if (!result)
                    {
                        return Results.BadRequest();
                    }

                    return Results.Ok("Success To Create Admin");

                }
                catch (Exception e)
                {

                    return Results.InternalServerError(e.Message);

                }
            });

            g.MapPost("/login", async (AuthServices services, LoginRequest receiver, IPasswordService p, JwtServices jwt) =>
            {
                try
                {
                    var req = await services.Login(receiver);
                    if (req == null)
                    {
                        return Results.Unauthorized();
                    }
                    var verified = await p.VerifyPasswordAsync(receiver.Password, req.Password);
                    if (!verified)
                    {
                        return Results.Unauthorized();
                    }
                    var token = jwt.GenerateToken(req);
                    return Results.Ok(new { Token = token });
                }
                catch (Exception e)
                {
                    return Results.InternalServerError(e.Message);
                    throw;
                }
            });
            app.MapGet("/antiforgery/token", (IAntiforgery antiforgery, HttpContext context) =>
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                // This sends back the actual string value and the field name it expects
                return Results.Ok(new { token = tokens.RequestToken, formKey = tokens.FormFieldName });
            });
        }//Function



    }//Class




}//Namespace
