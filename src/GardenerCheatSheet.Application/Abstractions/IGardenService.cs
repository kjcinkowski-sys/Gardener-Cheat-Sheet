using GardenerCheatSheet.Application.Dtos;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Manages the user's garden: adding plants (fetching + caching them from
/// Trefle on first use), listing them with resolved care information, applying
/// overrides, and recording waterings.
/// </summary>
public interface IGardenService
{
    Task<IReadOnlyList<GardenEntryDto>> GetGardenAsync(CancellationToken ct = default);

    Task<GardenEntryDto> AddAsync(AddGardenEntryRequest request, CancellationToken ct = default);

    Task<GardenEntryDto> AddCustomAsync(AddCustomPlantRequest request, CancellationToken ct = default);

    Task<GardenEntryDto?> UpdateAsync(int id, UpdateGardenEntryRequest request, CancellationToken ct = default);

    Task<bool> RemoveAsync(int id, CancellationToken ct = default);
}
