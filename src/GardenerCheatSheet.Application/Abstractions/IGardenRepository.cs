using GardenerCheatSheet.Domain.Garden;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Local persistence for <see cref="GardenEntry"/> rows. Entries are returned
/// with their <see cref="GardenEntry.Plant"/> loaded so care information can be
/// resolved without extra round-trips.
/// </summary>
public interface IGardenRepository
{
    Task<IReadOnlyList<GardenEntry>> GetAllAsync(CancellationToken ct = default);

    Task<GardenEntry?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<GardenEntry?> GetByTrefleIdAsync(int trefleId, CancellationToken ct = default);

    Task AddAsync(GardenEntry entry, CancellationToken ct = default);

    void Remove(GardenEntry entry);

    Task SaveChangesAsync(CancellationToken ct = default);
}
