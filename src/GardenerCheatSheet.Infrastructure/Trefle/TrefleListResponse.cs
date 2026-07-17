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
