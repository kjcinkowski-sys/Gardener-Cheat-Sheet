namespace GardenerCheatSheet.Application.Dtos;

/// <summary>
/// Partial update of a garden entry. Every field is optional; a null field is
/// left unchanged. To clear a value, send the matching Clear* flag.
/// The plant-identity fields (<see cref="DisplayName"/>, <see cref="ScientificName"/>,
/// <see cref="LightLevel"/>) apply only to custom plants and are ignored for
/// Trefle-sourced ones.
/// </summary>
public sealed record UpdateGardenEntryRequest
{
    public string? Nickname { get; init; }
    public string? Notes { get; init; }
    public DateOnly? LastWatered { get; init; }

    public int? WateringOverrideDays { get; init; }
    public bool ClearWateringOverride { get; init; }

    public bool? IsIndoorOverride { get; init; }

    /// <summary>The user's own photo URL. Send <see cref="ClearImage"/> to remove it.</summary>
    public string? ImageUrl { get; init; }
    public bool ClearImage { get; init; }

    // Custom-plant identity/care edits (ignored for Trefle plants).
    public string? DisplayName { get; init; }
    public string? ScientificName { get; init; }
    public int? LightLevel { get; init; }
    public bool ClearLightLevel { get; init; }
}
