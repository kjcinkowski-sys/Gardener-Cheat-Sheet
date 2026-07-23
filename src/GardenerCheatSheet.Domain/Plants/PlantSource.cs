namespace GardenerCheatSheet.Domain.Plants;

/// <summary>
/// Where a <see cref="Plant"/>'s data came from. Trefle plants have a stable
/// <see cref="Plant.TrefleId"/>; custom plants are user-created and have none.
/// </summary>
public enum PlantSource
{
    /// <summary>Sourced from the Trefle catalogue.</summary>
    Trefle = 0,

    /// <summary>Created by the user for something Trefle doesn't list.</summary>
    Custom = 1
}
