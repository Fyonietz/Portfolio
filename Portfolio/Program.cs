using Portfolio.Services;
using Portfolio.Controllers;
using Portfolio.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using DotNetEnv;


Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<Envs>(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer();
builder.Services.PostConfigure<JwtBearerOptions>(options =>
{
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")!))
    };
});
builder.Services.Configure<Envs>(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<AuthServices>();
builder.Services.AddScoped<JwtServices>();
builder.Services.AddScoped<IPasswordService,PasswordService>();

var app = builder.Build();
var envs = app.Services.GetRequiredService<IOptions<Envs>>().Value;
app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();

    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString()); // THIS IS THE IMPORTANT PART
        throw;
    }
    finally
    {
        sw.Stop();

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        Console.WriteLine(
            $"INFO: {ip} - \"{context.Request.Method} {context.Request.Path} " +
            $"{context.Response.StatusCode}\" {sw.ElapsedMilliseconds}ms"
        );
    }
});
app.UseAuthentication();
app.UseAuthorization();
app.MapAuth();

app.Run();


