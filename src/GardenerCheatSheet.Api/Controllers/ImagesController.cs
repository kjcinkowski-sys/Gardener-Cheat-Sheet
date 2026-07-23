using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GardenerCheatSheet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ImagesController : ControllerBase
{
    private const long MaxBytes = 5 * 1024 * 1024;

    // Allowed image content types mapped to the extension we store them under.
    private static readonly IReadOnlyDictionary<string, string> AllowedTypes =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif"
        };

    private readonly IImageStorage _storage;

    public ImagesController(IImageStorage storage) => _storage = storage;

    /// <summary>Upload a plant photo. Returns the URL to reference it by.</summary>
    [HttpPost]
    [RequestSizeLimit(MaxBytes + 1024 * 1024)]
    public async Task<ActionResult<UploadedImageDto>> Upload(IFormFile? file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "No file was uploaded." });
        }

        if (file.Length > MaxBytes)
        {
            return BadRequest(new { message = "Image must be 5 MB or smaller." });
        }

        if (!AllowedTypes.TryGetValue(file.ContentType, out var extension))
        {
            return BadRequest(new { message = "Only JPEG, PNG, WebP or GIF images are allowed." });
        }

        await using var stream = file.OpenReadStream();
        var url = await _storage.SaveAsync(stream, extension, ct);
        return Ok(new UploadedImageDto { Url = url });
    }
}
