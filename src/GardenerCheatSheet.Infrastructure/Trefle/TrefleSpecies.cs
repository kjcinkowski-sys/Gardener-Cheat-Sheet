using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

internal sealed class TrefleSpecies
{
    [JsonPropertyName("observations")]
    public string? Observations { get; set; }

    [JsonPropertyName("growth")]
    public TrefleGrowth? Growth { get; set; }
}
