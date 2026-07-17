namespace GardenerCheatSheet.Domain.Plants;

/// <summary>
/// Raw-ish growth facts sourced from Trefle that are useful for care advice
/// and for deriving a watering schedule. Any field may be unknown (null).
/// </summary>
public sealed record GrowthInfo
{
    /// <summary>Trefle light value on a 0–10 scale.</summary>
    public int? Light { get; init; }

    /// <summary>Trefle atmospheric humidity on a 0–10 scale.</summary>
    public int? AtmosphericHumidity { get; init; }

    /// <summary>Minimum annual precipitation in millimetres.</summary>
    public int? MinimumPrecipitationMm { get; init; }

    /// <summary>Maximum annual precipitation in millimetres.</summary>
    public int? MaximumPrecipitationMm { get; init; }

    public string? SoilTexture { get; init; }

    public string? Description { get; init; }

    public static GrowthInfo Empty { get; } = new();
}
