namespace GardenerCheatSheet.Api.Storage;

/// <summary>
/// Binds the <c>Storage</c> configuration section. <see cref="Provider"/> selects
/// where uploaded photos physically live: <c>Local</c> (disk, dev only) or
/// <c>S3</c> (any S3-compatible object store — Cloudflare R2, AWS S3, …).
/// </summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Either "Local" or "S3" (case-insensitive). Defaults to Local.</summary>
    public string Provider { get; set; } = "Local";

    public S3StorageOptions S3 { get; set; } = new();
}

/// <summary>
/// Settings for the S3-compatible object store. For Cloudflare R2, <see cref="ServiceUrl"/>
/// is <c>https://&lt;account-id&gt;.r2.cloudflarestorage.com</c> and <see cref="PublicBaseUrl"/>
/// is the bucket's public r2.dev URL (or a custom domain). Access/secret keys and the
/// public URL come from environment variables in production, never committed.
/// </summary>
public sealed class S3StorageOptions
{
    public string ServiceUrl { get; set; } = "";
    public string Bucket { get; set; } = "";
    public string AccessKey { get; set; } = "";
    public string SecretKey { get; set; } = "";

    /// <summary>Public base URL that objects are served from, e.g. <c>https://pub-xxxx.r2.dev</c>.</summary>
    public string PublicBaseUrl { get; set; } = "";

    /// <summary>Signing region. R2 accepts "auto"; AWS S3 wants a real region like "us-east-1".</summary>
    public string Region { get; set; } = "auto";
}
