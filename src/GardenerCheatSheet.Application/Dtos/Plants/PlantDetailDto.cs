namespace GardenerCheatSheet.Application.Dtos;

/// <summary>Full plant detail, including derived care information.</summary>
public sealed record PlantDetailDto
{
    public required int TrefleId { get; init; }
    public required string DisplayName { get; init; }
    public required string ScientificName { get; init; }
    public string? CommonName { get; init; }
    public string? Family { get; init; }
    public string? ImageUrl { get; init; }
    public string? Description { get; init; }
    public string? SoilTexture { get; init; }

    public required string LightRequirement { get; init; }
    public required bool IsIndoor { get; init; }
    public required WateringScheduleDto Watering { get; init; }
}
