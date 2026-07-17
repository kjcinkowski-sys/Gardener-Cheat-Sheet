namespace GardenerCheatSheet.Application.Dtos;

/// <summary>
/// Partial update of a garden entry. Every field is optional; a null field is
/// left unchanged. To clear the watering override, send
/// <see cref="ClearWateringOverride"/> = true.
/// </summary>
public sealed record UpdateGardenEntryRequest
{
    public string? Nickname { get; init; }
    public string? Notes { get; init; }
    public DateOnly? LastWatered { get; init; }
    public int? WateringOverrideDays { get; init; }
    public bool ClearWateringOverride { get; init; }
    public bool? IsIndoorOverride { get; init; }
}
