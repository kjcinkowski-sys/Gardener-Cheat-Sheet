using GardenerCheatSheet.Domain.Plants;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Local persistence for cached <see cref="Plant"/> species, keyed by their
/// Trefle id. Lets a garden entry reference stable plant data.
/// </summary>
public interface IPlantRepository
{
    Task<Plant?> GetByTrefleIdAsync(int trefleId, CancellationToken ct = default);

    Task<Plant?> GetByIdAsync(int id, CancellationToken ct = default);

    Task AddAsync(Plant plant, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
