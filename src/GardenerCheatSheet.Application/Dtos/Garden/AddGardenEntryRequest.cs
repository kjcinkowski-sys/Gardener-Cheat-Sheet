namespace GardenerCheatSheet.Application.Dtos;

/// <summary>Request to add a plant (by Trefle id) to the garden.</summary>
public sealed record AddGardenEntryRequest
{
    public required int TrefleId { get; init; }
    public string? Nickname { get; init; }
}
