using System.Text.Json.Serialization;

namespace GardenerCheatSheet.Infrastructure.Trefle;

internal sealed class TrefleDetailResponse
{
    [JsonPropertyName("data")]
    public TrefleDetail? Data { get; set; }
}
