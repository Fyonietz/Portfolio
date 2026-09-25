using Backend.Services;
using Backend.Models;
using Backend.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args);
Env.Value = builder.Configuration;
builder.Services.AddCors(options =>
{

    options.AddPolicy("AllowWebFrontend", policy =>
    {
        policy.WithOrigins(
           builder.Configuration["CORS"]
     )
     .AllowAnyMethod()
     .AllowAnyHeader()
     .AllowCredentials();
    });

});
// Add services to the container.
builder.Services.AddSingleton<Database>();
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization(Policies.Register);
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<IJWTService, JWTService>();

//CRUD Services Registration
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<ProfileServices>();
builder.Services.AddScoped<TechStackServices>();
builder.Services.AddScoped<ProjectServices>();
builder.Services.AddScoped<SkillServices>();
builder.Services.AddScoped<ContactServices>();
builder.Services.AddScoped<AchievementServices>();
var app = builder.Build();

app.UseCors("AllowWebFrontend"); 
//Logger
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        throw;
    }
    finally
    {
        sw.Stop();

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        Console.WriteLine(
            $"INFO: {ip} - \"{context.Request.Method} {context.Request.Path} {context.Response.StatusCode}\" {sw.ElapsedMilliseconds}ms"
        );
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
//CRUD Controller Registration
app.MapAuth();

//Project
app.MapProject();
app.MapTechStack();
app.MapSkill();
// Profile & Contact endpoints
app.MapProfile();
app.MapContact();
app.MapAchievement();
app.Run();
