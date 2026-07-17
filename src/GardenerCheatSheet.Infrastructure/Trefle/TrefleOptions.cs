namespace GardenerCheatSheet.Infrastructure.Trefle;

/// <summary>
/// Configuration for the Trefle API client. Bind from the "Trefle" config
/// section. Get a free token by registering at https://trefle.io.
/// </summary>
public sealed class TrefleOptions
{
    public const string SectionName = "Trefle";

    public string BaseUrl { get; set; } = "https://trefle.io/api/v1/";

    public string Token { get; set; } = string.Empty;

    /// <summary>How long to cache Trefle responses in memory.</summary>
    public int CacheMinutes { get; set; } = 30;
}
