namespace GardenerCheatSheet.Application.Dtos;

public sealed record PlantSearchResultDto
{
    public required IReadOnlyList<PlantSummaryDto> Plants { get; init; }
    public int Page { get; init; }
    public int? TotalPages { get; init; }
}
