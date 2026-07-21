using BellaSposaBridal.Application;
using BellaSposaBridal.Infrastructure;
using BellaSposaBridal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
    o.Limits.MaxRequestBodySize = 150_000_000); // 150 MB

// Multipart form limit defaults to 128 MiB — below the Kestrel limit above.
// Lift it so uploads are governed by Kestrel / [RequestSizeLimit] only.
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
    o.MultipartBodyLengthLimit = long.MaxValue);

// Railway provides DATABASE_URL as postgresql://user:pass@host:port/db
// Parse it into the Npgsql connection string format
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
Console.WriteLine($"[Startup] DATABASE_URL present: {!string.IsNullOrEmpty(databaseUrl)}");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var connStr = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    Console.WriteLine($"[Startup] Connecting to: {uri.Host}:{uri.Port}");
    builder.Configuration["ConnectionStrings:DefaultConnection"] = connStr;
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Bella Sposa Bridal API";
        options.Theme = ScalarTheme.Moon;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors();

// Admin auth middleware
app.Use(async (context, next) =>
{
    if (RequiresAdminAuth(context))
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var adminToken = context.RequestServices
            .GetRequiredService<IConfiguration>()["AdminToken"] ?? "bella-sposa-admin-2024";

        if (string.IsNullOrEmpty(authHeader)
            || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            || authHeader["Bearer ".Length..] != adminToken)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { message = "Unauthorized" });
            return;
        }
    }
    await next();
});

app.MapControllers();

app.Run();

static bool RequiresAdminAuth(HttpContext ctx)
{
    var method = ctx.Request.Method.ToUpperInvariant();
    var path   = ctx.Request.Path.Value ?? "";

    // Dress admin routes
    if (path.Contains("/dresses", StringComparison.OrdinalIgnoreCase))
    {
        if (method == "GET"    && path.EndsWith("/admin", StringComparison.OrdinalIgnoreCase)) return true;
        if (method == "POST"   && (path.EndsWith("/dresses", StringComparison.OrdinalIgnoreCase)
                                || path.EndsWith("/photos", StringComparison.OrdinalIgnoreCase)
                                || path.EndsWith("/videos", StringComparison.OrdinalIgnoreCase))) return true;
        if (method is "PUT" or "PATCH" or "DELETE") return true;
    }

    // Collection admin routes (only mutations)
    if (path.Contains("/collections", StringComparison.OrdinalIgnoreCase))
    {
        if (method is "POST" or "PUT" or "PATCH" or "DELETE") return true;
    }

    // Upload
    if (method == "POST" && path.Contains("/upload", StringComparison.OrdinalIgnoreCase)) return true;

    // Silhouette admin routes
    if (path.Contains("/silhouettes", StringComparison.OrdinalIgnoreCase))
    {
        if (method is "POST" or "DELETE") return true;
    }

    // Appointment type admin routes
    if (path.Contains("/appointment-types", StringComparison.OrdinalIgnoreCase))
    {
        if (method is "POST" or "PUT" or "PATCH" or "DELETE") return true;
    }

    // Schedule admin routes
    if (path.Contains("/schedule", StringComparison.OrdinalIgnoreCase))
    {
        if (method is "PUT" or "PATCH" or "DELETE") return true;
        if (method == "GET" && path.Contains("/day/", StringComparison.OrdinalIgnoreCase)) return true;
    }

    // Appointments admin routes
    if (path.Contains("/appointments", StringComparison.OrdinalIgnoreCase))
    {
        if (method == "GET" && !path.Contains("/booked-slots", StringComparison.OrdinalIgnoreCase)) return true;
        if (method is "PATCH" or "DELETE") return true;
    }

    return false;
}
