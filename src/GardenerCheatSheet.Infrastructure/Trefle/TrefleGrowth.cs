using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

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
