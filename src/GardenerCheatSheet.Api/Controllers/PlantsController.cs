using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GardenerCheatSheet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PlantsController : ControllerBase
{
    private readonly IPlantCatalogService _catalog;

    public PlantsController(IPlantCatalogService catalog) => _catalog = catalog;

    /// <summary>Search the plant catalog (Trefle) by free text.</summary>
    [HttpGet]
    public async Task<ActionResult<PlantSearchResultDto>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        CancellationToken ct = default)
    {
        var result = await _catalog.SearchAsync(search ?? string.Empty, page, ct);
        return Ok(result);
    }

    /// <summary>Get full detail and derived care info for one plant.</summary>
    [HttpGet("{trefleId:int}")]
    public async Task<ActionResult<PlantDetailDto>> GetById(int trefleId, CancellationToken ct = default)
    {
        var plant = await _catalog.GetDetailAsync(trefleId, ct);
        return plant is null ? NotFound() : Ok(plant);
    }
}
