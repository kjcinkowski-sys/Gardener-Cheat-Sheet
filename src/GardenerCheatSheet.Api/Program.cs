using GardenerCheatSheet.Api.Storage;
using GardenerCheatSheet.Application;
using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Infrastructure;
using GardenerCheatSheet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string AngularCorsPolicy = "AngularDev";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// User-photo storage is a web concern (served from wwwroot), so it lives in the API layer.
builder.Services.AddScoped<IImageStorage, LocalImageStorage>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularCorsPolicy, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? new[] { "http://localhost:4200" };
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply any pending EF migrations on startup, creating the SQLite schema on a
// fresh database. (Existing dev databases created by the old EnsureCreated path
// have no migrations history; delete gardener.db once to let migrations take over.)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Ensure the uploads folder exists so user-photo storage has somewhere to write.
var uploadsRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads", "plants");
Directory.CreateDirectory(uploadsRoot);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AngularCorsPolicy);

// Serve user-uploaded plant photos from wwwroot (e.g. /uploads/plants/{file}).
app.UseStaticFiles();

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
