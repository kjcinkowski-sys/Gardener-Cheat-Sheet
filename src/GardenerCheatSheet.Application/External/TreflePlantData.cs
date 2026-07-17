namespace GardenerCheatSheet.Application.External;

/// <summary>
/// A provider-neutral snapshot of a plant as returned by
/// <see cref="Abstractions.ITreflePlantProvider"/>. Infrastructure maps
/// Trefle's raw JSON into this shape so the rest of the app never depends on
/// Trefle's wire format.
/// </summary>
public sealed record TreflePlantData
{
    public required int TrefleId { get; init; }
    public required string ScientificName { get; init; }
    public string? CommonName { get; init; }
    public string? Family { get; init; }
    public string? ImageUrl { get; init; }

    public int? Light { get; init; }
    public int? AtmosphericHumidity { get; init; }
    public int? MinimumPrecipitationMm { get; init; }
    public int? MaximumPrecipitationMm { get; init; }
    public string? SoilTexture { get; init; }
    public string? Description { get; init; }
}

/// <summary>A page of search results from the provider.</summary>
public sealed record TrefleSearchResult
{
    public required IReadOnlyList<TreflePlantData> Plants { get; init; }
    public int Page { get; init; }
    public int? TotalPages { get; init; }
}
