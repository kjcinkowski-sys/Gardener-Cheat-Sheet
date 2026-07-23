using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Common;
using GardenerCheatSheet.Application.Dtos;
using GardenerCheatSheet.Application.Mapping;
using GardenerCheatSheet.Domain.Garden;
using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Application.Services;

/// <inheritdoc cref="IGardenService" />
public sealed class GardenService : IGardenService
{
    private readonly IGardenRepository _garden;
    private readonly IPlantRepository _plants;
    private readonly ITreflePlantProvider _provider;
    private readonly IWateringScheduleCalculator _wateringCalculator;
    private readonly TimeProvider _clock;

    public GardenService(
        IGardenRepository garden,
        IPlantRepository plants,
        ITreflePlantProvider provider,
        IWateringScheduleCalculator wateringCalculator,
        TimeProvider clock)
    {
        _garden = garden;
        _plants = plants;
        _provider = provider;
        _wateringCalculator = wateringCalculator;
        _clock = clock;
    }

    private DateOnly Today => DateOnly.FromDateTime(_clock.GetLocalNow().DateTime);

    public async Task<IReadOnlyList<GardenEntryDto>> GetGardenAsync(CancellationToken ct = default)
    {
        var entries = await _garden.GetAllAsync(ct);
        var today = Today;
        return entries.Select(e => ToDto(e, today)).ToList();
    }

    public async Task<GardenEntryDto> AddAsync(AddGardenEntryRequest request, CancellationToken ct = default)
    {
        // Idempotent: adding a plant already in the garden returns the existing entry.
        var existing = await _garden.GetByTrefleIdAsync(request.TrefleId, ct);
        if (existing is not null)
        {
            return ToDto(existing, Today);
        }

        var plant = await EnsurePlantCachedAsync(request.TrefleId, ct);

        var entry = new GardenEntry(plant, request.Nickname, Today);
        await _garden.AddAsync(entry, ct);
        await _garden.SaveChangesAsync(ct);

        return ToDto(entry, Today);
    }

    public async Task<GardenEntryDto> AddCustomAsync(AddCustomPlantRequest request, CancellationToken ct = default)
    {
        var growth = new GrowthInfo { Light = request.LightLevel };
        var plant = Plant.CreateCustom(
            displayName: request.DisplayName,
            scientificName: request.ScientificName,
            family: null,
            imageUrl: null,
            growth: growth,
            isIndoor: request.IsIndoor);

        // Persist the plant first so its surrogate key is available for the entry,
        // mirroring EnsurePlantCachedAsync.
        await _plants.AddAsync(plant, ct);
        await _plants.SaveChangesAsync(ct);

        var entry = new GardenEntry(plant, request.Nickname, Today);
        if (request.WateringIntervalDays is int days)
        {
            entry.OverrideWatering(days);
        }
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            entry.SetNotes(request.Notes);
        }
        if (!string.IsNullOrWhiteSpace(request.ImageUrl))
        {
            entry.SetImageUrl(request.ImageUrl);
        }

        await _garden.AddAsync(entry, ct);
        await _garden.SaveChangesAsync(ct);

        return ToDto(entry, Today);
    }

    public async Task<GardenEntryDto?> UpdateAsync(int id, UpdateGardenEntryRequest request, CancellationToken ct = default)
    {
        var entry = await _garden.GetByIdAsync(id, ct);
        if (entry is null)
        {
            return null;
        }

        if (request.Nickname is not null)
        {
            entry.Rename(request.Nickname);
        }

        if (request.Notes is not null)
        {
            entry.SetNotes(request.Notes);
        }

        if (request.LastWatered is DateOnly watered)
        {
            entry.MarkWatered(watered);
        }

        if (request.ClearWateringOverride)
        {
            entry.OverrideWatering(null);
        }
        else if (request.WateringOverrideDays is int days)
        {
            entry.OverrideWatering(days);
        }

        if (request.IsIndoorOverride is bool indoor)
        {
            entry.OverrideIndoor(indoor);
        }

        if (request.ClearImage)
        {
            entry.SetImageUrl(null);
        }
        else if (request.ImageUrl is not null)
        {
            entry.SetImageUrl(request.ImageUrl);
        }

        ApplyCustomPlantEdits(entry.Plant, request);

        await _garden.SaveChangesAsync(ct);
        return ToDto(entry, Today);
    }

    /// <summary>
    /// Applies identity/light edits to a custom plant. No-op for Trefle plants
    /// (their data is owned by the source) and when no such fields were sent.
    /// </summary>
    private static void ApplyCustomPlantEdits(Plant plant, UpdateGardenEntryRequest request)
    {
        if (plant.Source != PlantSource.Custom)
        {
            return;
        }

        var touchesIdentity = request.DisplayName is not null
                              || request.ScientificName is not null
                              || request.LightLevel is not null
                              || request.ClearLightLevel;
        if (!touchesIdentity)
        {
            return;
        }

        var light = request.ClearLightLevel ? null : request.LightLevel ?? plant.Growth.Light;
        var growth = plant.Growth with { Light = light };

        var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
            ? plant.DisplayName
            : request.DisplayName!;
        var scientificName = request.ScientificName ?? plant.ScientificName;

        plant.EditCustomDetails(displayName, scientificName, growth);
    }

    public async Task<bool> RemoveAsync(int id, CancellationToken ct = default)
    {
        var entry = await _garden.GetByIdAsync(id, ct);
        if (entry is null)
        {
            return false;
        }

        _garden.Remove(entry);
        await _garden.SaveChangesAsync(ct);
        return true;
    }

    private async Task<Plant> EnsurePlantCachedAsync(int trefleId, CancellationToken ct)
    {
        var cached = await _plants.GetByTrefleIdAsync(trefleId, ct);
        if (cached is not null)
        {
            return cached;
        }

        var data = await _provider.GetByIdAsync(trefleId, ct)
                   ?? throw new PlantNotFoundException(trefleId);

        var plant = PlantMapper.ToDomain(data);
        await _plants.AddAsync(plant, ct);
        await _plants.SaveChangesAsync(ct);
        return plant;
    }

    private GardenEntryDto ToDto(GardenEntry entry, DateOnly today)
    {
        var derived = _wateringCalculator.Derive(entry.Plant);
        var resolved = entry.ResolveSchedule(derived);
        return PlantMapper.ToDto(entry, resolved, today);
    }
}
