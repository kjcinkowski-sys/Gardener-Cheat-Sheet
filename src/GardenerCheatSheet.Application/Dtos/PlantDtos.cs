namespace GardenerCheatSheet.Application.Dtos;

/// <summary>Lightweight plant shape for search/list views.</summary>
public sealed record PlantSummaryDto
{
    public required int TrefleId { get; init; }
    public required string DisplayName { get; init; }
    public string? ScientificName { get; init; }
    public string? ImageUrl { get; init; }
    public required bool IsIndoor { get; init; }
}

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

public sealed record WateringScheduleDto
{
    public required string Category { get; init; }
    public required int DaysBetweenWatering { get; init; }
    public required string Source { get; init; }
}

public sealed record PlantSearchResultDto
{
    public required IReadOnlyList<PlantSummaryDto> Plants { get; init; }
    public int Page { get; init; }
    public int? TotalPages { get; init; }
}
