namespace GardenerCheatSheet.Application.Common;

/// <summary>
/// Thrown when a plant referenced by a Trefle id cannot be found in Trefle.
/// </summary>
public sealed class PlantNotFoundException : Exception
{
    public PlantNotFoundException(int trefleId)
        : base($"No plant was found with Trefle id {trefleId}.")
    {
        TrefleId = trefleId;
    }

    public int TrefleId { get; }
}
