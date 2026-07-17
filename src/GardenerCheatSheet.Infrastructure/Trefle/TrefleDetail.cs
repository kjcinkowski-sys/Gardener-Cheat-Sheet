using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

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
