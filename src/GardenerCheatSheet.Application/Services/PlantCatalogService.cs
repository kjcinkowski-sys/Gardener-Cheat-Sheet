using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Dtos;
using GardenerCheatSheet.Application.Mapping;

namespace GardenerCheatSheet.Application.Services;

/// <inheritdoc cref="IPlantCatalogService" />
public sealed class PlantCatalogService : IPlantCatalogService
{
    private readonly ITreflePlantProvider _provider;
    private readonly IWateringScheduleCalculator _wateringCalculator;

    public PlantCatalogService(
        ITreflePlantProvider provider,
        IWateringScheduleCalculator wateringCalculator)
    {
        _provider = provider;
        _wateringCalculator = wateringCalculator;
    }

    public async Task<PlantSearchResultDto> SearchAsync(string query, int page, CancellationToken ct = default)
    {
        if (page < 1)
        {
            page = 1;
        }

        var result = await _provider.SearchAsync(query ?? string.Empty, page, ct);

        return new PlantSearchResultDto
        {
            Plants = result.Plants.Select(PlantMapper.ToSummary).ToList(),
            Page = result.Page,
            TotalPages = result.TotalPages
        };
    }

    public async Task<PlantDetailDto?> GetDetailAsync(int trefleId, CancellationToken ct = default)
    {
        var data = await _provider.GetByIdAsync(trefleId, ct);
        if (data is null)
        {
            return null;
        }

        var plant = PlantMapper.ToDomain(data);
        var watering = _wateringCalculator.Derive(plant);
        return PlantMapper.ToDetail(plant, watering);
    }
}
