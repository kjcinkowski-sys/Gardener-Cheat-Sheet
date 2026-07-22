using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Application.Common;
using GardenerCheatSheet.Application.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GardenerCheatSheet.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GardenController : ControllerBase
{
    private readonly IGardenService _garden;

    public GardenController(IGardenService garden) => _garden = garden;

    /// <summary>List the plants in the user's garden with resolved care info.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GardenEntryDto>>> GetGarden(CancellationToken ct = default)
    {
        var entries = await _garden.GetGardenAsync(ct);
        return Ok(entries);
    }

    /// <summary>Add a plant (by Trefle id) to the garden.</summary>
    [HttpPost]
    public async Task<ActionResult<GardenEntryDto>> Add(
        [FromBody] AddGardenEntryRequest request,
        CancellationToken ct = default)
    {
        try
        {
            var entry = await _garden.AddAsync(request, ct);
            return CreatedAtAction(nameof(GetGarden), routeValues: null, value: entry);
        }
        catch (PlantNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Add a user-created custom plant (not in Trefle) to the garden.</summary>
    [HttpPost("custom")]
    public async Task<ActionResult<GardenEntryDto>> AddCustom(
        [FromBody] AddCustomPlantRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return BadRequest(new { message = "A plant name is required." });
        }

        var entry = await _garden.AddCustomAsync(request, ct);
        return CreatedAtAction(nameof(GetGarden), routeValues: null, value: entry);
    }

    /// <summary>Update a garden entry (nickname, notes, watering override, etc.).</summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<GardenEntryDto>> Update(
        int id,
        [FromBody] UpdateGardenEntryRequest request,
        CancellationToken ct = default)
    {
        var updated = await _garden.UpdateAsync(id, request, ct);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Remove a plant from the garden.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Remove(int id, CancellationToken ct = default)
    {
        var removed = await _garden.RemoveAsync(id, ct);
        return removed ? NoContent() : NotFound();
    }
}
