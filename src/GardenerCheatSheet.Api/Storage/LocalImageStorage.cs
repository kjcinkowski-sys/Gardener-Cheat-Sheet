using GardenerCheatSheet.Application.Abstractions;

namespace GardenerCheatSheet.Api.Storage;

/// <summary>
/// Stores uploaded images on the local filesystem under
/// <c>wwwroot/uploads/plants</c>, served back as static files. Filenames are
/// server-generated GUIDs so a client-supplied name can never influence the path.
/// </summary>
public sealed class LocalImageStorage : IImageStorage
{
    private const string RelativeDir = "uploads/plants";

    private readonly IWebHostEnvironment _env;

    public LocalImageStorage(IWebHostEnvironment env) => _env = env;

    public async Task<string> SaveAsync(Stream content, string fileExtension, CancellationToken ct = default)
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var dir = Path.Combine(webRoot, "uploads", "plants");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid():N}{fileExtension}";
        var fullPath = Path.Combine(dir, fileName);

        await using (var file = File.Create(fullPath))
        {
            await content.CopyToAsync(file, ct);
        }

        return $"/{RelativeDir}/{fileName}";
    }
}
