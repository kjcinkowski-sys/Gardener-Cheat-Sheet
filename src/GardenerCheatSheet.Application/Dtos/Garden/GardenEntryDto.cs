namespace GardenerCheatSheet.Application.Dtos;

/// <summary>A plant in the user's garden, with resolved care information.</summary>
public sealed record GardenEntryDto
{
    public required int Id { get; init; }
    public required int TrefleId { get; init; }
    public required string DisplayName { get; init; }
    public string? Nickname { get; init; }
    public string? ScientificName { get; init; }
    public string? ImageUrl { get; init; }
    public string? Notes { get; init; }

    public required bool IsIndoor { get; init; }
    public required string LightRequirement { get; init; }
    public required WateringScheduleDto Watering { get; init; }

    public DateOnly DateAdded { get; init; }
    public DateOnly? LastWatered { get; init; }
    public DateOnly? NextWateringDate { get; init; }
    public required bool IsDue { get; init; }
}
