using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Application.Abstractions;

/// <summary>
/// Derives a default <see cref="WateringSchedule"/> for a plant from its Trefle
/// growth data. Trefle exposes no explicit watering field, so this encapsulates
/// the heuristic that turns precipitation/humidity/light into a cadence.
/// </summary>
public interface IWateringScheduleCalculator
{
    WateringSchedule Derive(Plant plant);
}
