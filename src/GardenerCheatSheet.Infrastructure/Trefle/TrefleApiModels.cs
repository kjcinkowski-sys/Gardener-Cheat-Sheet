using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

// Minimal, defensive models for the subset of the Trefle JSON we consume.
// Every field is nullable so a missing/renamed field never breaks deserialisation.

internal sealed class TrefleListResponse
{
    [JsonPropertyName("data")]
    public List<TrefleListItem>? Data { get; set; }

    [JsonPropertyName("meta")]
    public TrefleMeta? Meta { get; set; }
}

internal sealed class TrefleMeta
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}

internal sealed class TrefleListItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("common_name")]
    public string? CommonName { get; set; }

    [JsonPropertyName("scientific_name")]
    public string? ScientificName { get; set; }

    [JsonPropertyName("family_common_name")]
    public string? FamilyCommonName { get; set; }

    [JsonPropertyName("family")]
    public string? Family { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }
}

internal sealed class TrefleDetailResponse
{
    [JsonPropertyName("data")]
    public TrefleDetail? Data { get; set; }
}

internal sealed class TrefleDetail
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("common_name")]
    public string? CommonName { get; set; }

    [JsonPropertyName("scientific_name")]
    public string? ScientificName { get; set; }

    [JsonPropertyName("family_common_name")]
    public string? FamilyCommonName { get; set; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("observations")]
    public string? Observations { get; set; }

    [JsonPropertyName("main_species")]
    public TrefleSpecies? MainSpecies { get; set; }
}

internal sealed class TrefleSpecies
{
    [JsonPropertyName("observations")]
    public string? Observations { get; set; }

    [JsonPropertyName("growth")]
    public TrefleGrowth? Growth { get; set; }
}

internal sealed class TrefleGrowth
{
    [JsonPropertyName("light")]
    public int? Light { get; set; }

    [JsonPropertyName("atmospheric_humidity")]
    public int? AtmosphericHumidity { get; set; }

    [JsonPropertyName("soil_texture")]
    public int? SoilTexture { get; set; }

    [JsonPropertyName("minimum_precipitation")]
    public TrefleDistance? MinimumPrecipitation { get; set; }

    [JsonPropertyName("maximum_precipitation")]
    public TrefleDistance? MaximumPrecipitation { get; set; }
}

internal sealed class TrefleDistance
{
    [JsonPropertyName("mm")]
    public double? Mm { get; set; }
}
