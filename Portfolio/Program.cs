using Portfolio.Services;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddSingleton<Database>();
var app = builder.Build();

app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    await next();
    sw.Stop();

    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    Console.WriteLine(
        $"INFO: {ip} - \"{context.Request.Method} {context.Request.Path} " +
        $"{context.Response.StatusCode}\" {sw.ElapsedMilliseconds}ms"
    );
});

app.Run();


