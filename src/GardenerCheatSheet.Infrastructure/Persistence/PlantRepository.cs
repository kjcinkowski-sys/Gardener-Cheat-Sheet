using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Domain.Plants;
using Microsoft.EntityFrameworkCore;

namespace GardenerCheatSheet.Infrastructure.Persistence;

public sealed class PlantRepository : IPlantRepository
{
    private readonly AppDbContext _db;

    public PlantRepository(AppDbContext db) => _db = db;

    public Task<Plant?> GetByTrefleIdAsync(int trefleId, CancellationToken ct = default) =>
        _db.Plants.FirstOrDefaultAsync(p => p.TrefleId == trefleId, ct);

    public async Task<Plant?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Plants.FindAsync([id], ct);

    public async Task AddAsync(Plant plant, CancellationToken ct = default) =>
        await _db.Plants.AddAsync(plant, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}
