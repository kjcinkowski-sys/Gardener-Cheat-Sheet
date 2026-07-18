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
