namespace GardenerCheatSheet.Application.Dtos;

/// <summary>
/// Request to add a user-created plant (one Trefle doesn't list) directly to the
/// garden. Only <see cref="DisplayName"/> is required; care fields are optional
/// and reuse the garden entry's override mechanisms.
/// </summary>
public sealed record AddCustomPlantRequest
{
    public required string DisplayName { get; init; }
    public string? ScientificName { get; init; }
    public string? Nickname { get; init; }

    /// <summary>URL of an already-uploaded photo (see the images endpoint).</summary>
    public string? ImageUrl { get; init; }

    public bool IsIndoor { get; init; }

    /// <summary>How often to water, in days. Null uses the derived default.</summary>
    public int? WateringIntervalDays { get; init; }

    /// <summary>Light need on Trefle's 0–10 scale (e.g. 2 shade, 5 partial, 8 sun).</summary>
    public int? LightLevel { get; init; }

    public string? Notes { get; init; }
}
