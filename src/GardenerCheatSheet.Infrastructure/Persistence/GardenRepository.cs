using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Domain.Garden;
using Microsoft.EntityFrameworkCore;

namespace GardenerCheatSheet.Infrastructure.Persistence;

public sealed class GardenRepository : IGardenRepository
{
    private readonly AppDbContext _db;

    public GardenRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<GardenEntry>> GetAllAsync(CancellationToken ct = default) =>
        await _db.GardenEntries
            .Include(e => e.Plant)
            .AsNoTracking()
            .OrderBy(e => e.DateAdded)
            .ToListAsync(ct);

    public Task<GardenEntry?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.GardenEntries
            .Include(e => e.Plant)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<GardenEntry?> GetByTrefleIdAsync(int trefleId, CancellationToken ct = default) =>
        _db.GardenEntries
            .Include(e => e.Plant)
            .FirstOrDefaultAsync(e => e.Plant.TrefleId == trefleId, ct);

    public async Task AddAsync(GardenEntry entry, CancellationToken ct = default) =>
        await _db.GardenEntries.AddAsync(entry, ct);

    public void Remove(GardenEntry entry) => _db.GardenEntries.Remove(entry);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
