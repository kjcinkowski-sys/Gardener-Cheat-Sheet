using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

internal sealed class TrefleDistance
{
    [JsonPropertyName("mm")]
    public double? Mm { get; set; }
}
