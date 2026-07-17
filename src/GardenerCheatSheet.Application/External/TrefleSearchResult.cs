namespace GardenerCheatSheet.Application.External;

/// <summary>A page of search results from the provider.</summary>
public sealed record TrefleSearchResult
{
    public required IReadOnlyList<TreflePlantData> Plants { get; init; }
    public int Page { get; init; }
    public int? TotalPages { get; init; }
}
