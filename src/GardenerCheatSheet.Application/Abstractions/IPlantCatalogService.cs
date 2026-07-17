using GardenerCheatSheet.Application.Dtos;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Browse and inspect the plant catalog. Talks to Trefle via
/// <see cref="ITreflePlantProvider"/> and enriches results with derived care
/// information (watering, light, indoor/outdoor).
/// </summary>
public interface IPlantCatalogService
{
    Task<PlantSearchResultDto> SearchAsync(string query, int page, CancellationToken ct = default);

    Task<PlantDetailDto?> GetDetailAsync(int trefleId, CancellationToken ct = default);
}
