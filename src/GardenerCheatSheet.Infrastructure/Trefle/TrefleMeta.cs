using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

internal sealed class TrefleMeta
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }
}
