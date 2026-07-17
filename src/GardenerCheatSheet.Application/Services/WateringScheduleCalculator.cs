using GardenerCheatSheet.Application.Abstractions;
using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Application.Services;

/// <summary>
/// Default <see cref="IWateringScheduleCalculator"/>. Trefle has no watering
/// field, so this infers a cadence from the growth data that is available,
/// preferring precipitation, then falling back to light exposure.
/// </summary>
public sealed class WateringScheduleCalculator : IWateringScheduleCalculator
{
    // Cadence, in days, for each thirst category.
    private const int LowCadenceDays = 14;
    private const int MediumCadenceDays = 7;
    private const int HighCadenceDays = 3;

    public WateringSchedule Derive(Plant plant)
    {
        var category = InferCategory(plant.Growth);
        var days = category switch
        {
            WateringCategory.Low => LowCadenceDays,
            WateringCategory.High => HighCadenceDays,
            _ => MediumCadenceDays
        };

        // Unknown collapses to the medium cadence but keeps a Medium category so
        // the UI shows a sensible label.
        var reported = category == WateringCategory.Unknown ? WateringCategory.Medium : category;
        return new WateringSchedule(reported, days, WateringSource.Derived);
    }

    private static WateringCategory InferCategory(GrowthInfo growth)
    {
        // Precipitation is the strongest signal: a plant native to a wet climate
        // is thirstier than a desert species.
        if (growth.MinimumPrecipitationMm is int min && growth.MaximumPrecipitationMm is int max)
        {
            var avg = (min + max) / 2.0;
            return avg switch
            {
                < 500 => WateringCategory.Low,
                <= 1500 => WateringCategory.Medium,
                _ => WateringCategory.High
            };
        }

        // Fall back to light: full-sun plants dry out faster than shade plants.
        return LightRequirementExtensions.FromTrefleLight(growth.Light) switch
        {
            LightRequirement.FullSun => WateringCategory.High,
            LightRequirement.PartialShade => WateringCategory.Medium,
            LightRequirement.Shade => WateringCategory.Low,
            _ => WateringCategory.Unknown
        };
    }
}
