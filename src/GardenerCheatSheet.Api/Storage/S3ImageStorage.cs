using Amazon.S3;
using Amazon.S3.Model;
using GardenerCheatSheet.Application.Abstractions;
using Microsoft.Extensions.Options;

namespace GardenerCheatSheet.Api.Storage;

/// <summary>
/// Stores uploaded images in an S3-compatible object store (Cloudflare R2 / AWS S3)
/// and returns their public absolute URL. Object keys are server-generated GUIDs under
/// <c>plants/</c> so a client-supplied name can never influence the key.
/// </summary>
public sealed class S3ImageStorage : IImageStorage
{
    private const string KeyPrefix = "plants";

    // The controller validated the extension already; map it back to a content type so
    // the object is served with the right header.
    private static readonly IReadOnlyDictionary<string, string> ContentTypeByExtension =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp",
            [".gif"] = "image/gif"
        };

    private readonly IAmazonS3 _client;
    private readonly S3StorageOptions _options;

    public S3ImageStorage(IAmazonS3 client, IOptions<StorageOptions> options)
    {
        _client = client;
        _options = options.Value.S3;
    }

    public async Task<string> SaveAsync(Stream content, string fileExtension, CancellationToken ct = default)
    {
        var key = $"{KeyPrefix}/{Guid.NewGuid():N}{fileExtension}";

        var request = new PutObjectRequest
        {
            BucketName = _options.Bucket,
            Key = key,
            InputStream = content,
            ContentType = ContentTypeByExtension.TryGetValue(fileExtension, out var type)
                ? type
                : "application/octet-stream",
            // R2 does not support the SDK's streaming (chunked) payload signing; disable it.
            DisablePayloadSigning = true
        };

        await _client.PutObjectAsync(request, ct);

        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/{key}";
    }
}
