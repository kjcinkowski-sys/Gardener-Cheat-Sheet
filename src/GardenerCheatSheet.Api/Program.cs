using Amazon.Runtime;
using Amazon.S3;
using GardenerCheatSheet.Api.Storage;
using GardenerCheatSheet.Application;
using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Infrastructure;
using GardenerCheatSheet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

const string AngularCorsPolicy = "AngularDev";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// User-photo storage. Local disk for dev; an S3-compatible object store (Cloudflare R2)
// for deployment, since app hosts have ephemeral/non-shared disks. Selected by config.
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection(StorageOptions.SectionName));
var storageProvider = builder.Configuration[$"{StorageOptions.SectionName}:Provider"] ?? "Local";

if (string.Equals(storageProvider, "S3", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IAmazonS3>(sp =>
    {
        var s3 = sp.GetRequiredService<IOptions<StorageOptions>>().Value.S3;
        var config = new AmazonS3Config
        {
            ServiceURL = s3.ServiceUrl,
            ForcePathStyle = true,          // required for R2 and most non-AWS S3 stores
            AuthenticationRegion = s3.Region
        };
        return new AmazonS3Client(new BasicAWSCredentials(s3.AccessKey, s3.SecretKey), config);
    });
    builder.Services.AddScoped<IImageStorage, S3ImageStorage>();
}
else
{
    builder.Services.AddScoped<IImageStorage, LocalImageStorage>();
}

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

// With local storage, ensure the uploads folder exists so photos have somewhere to write.
// (S3/R2 needs none of this — objects live in the bucket and are served from its public URL.)
var usingLocalStorage = !string.Equals(storageProvider, "S3", StringComparison.OrdinalIgnoreCase);
if (usingLocalStorage)
{
    var uploadsRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads", "plants");
    Directory.CreateDirectory(uploadsRoot);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(AngularCorsPolicy);

// Serve user-uploaded plant photos from wwwroot (e.g. /uploads/plants/{file}) when using
// local storage. With S3/R2 the photos are served from the bucket's public URL instead.
if (usingLocalStorage)
{
    app.UseStaticFiles();
}

app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
