using GardenerCheatSheet.Application.Services;
using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;
using Xunit;

namespace GardenerCheatSheet.UnitTests;

public class WateringScheduleCalculatorTests
{
    private readonly WateringScheduleCalculator _calculator = new();

    private static Plant PlantWith(GrowthInfo growth) =>
        new(trefleId: 1, scientificName: "Test plant", commonName: null, family: null,
            imageUrl: null, growth: growth, isIndoor: false);

    [Fact]
    public void Derive_HighPrecipitation_IsThirsty()
    {
        var plant = PlantWith(new GrowthInfo { MinimumPrecipitationMm = 1600, MaximumPrecipitationMm = 2000 });

        var schedule = _calculator.Derive(plant);

        Assert.Equal(WateringCategory.High, schedule.Category);
        Assert.Equal(3, schedule.DaysBetweenWatering);
        Assert.Equal(WateringSource.Derived, schedule.Source);
    }

    [Fact]
    public void Derive_LowPrecipitation_IsDroughtTolerant()
    {
        var plant = PlantWith(new GrowthInfo { MinimumPrecipitationMm = 100, MaximumPrecipitationMm = 300 });

        var schedule = _calculator.Derive(plant);

        Assert.Equal(WateringCategory.Low, schedule.Category);
        Assert.Equal(14, schedule.DaysBetweenWatering);
    }

    [Fact]
    public void Derive_FallsBackToLight_WhenNoPrecipitation()
    {
        var plant = PlantWith(new GrowthInfo { Light = 9 });

        var schedule = _calculator.Derive(plant);

        Assert.Equal(WateringCategory.High, schedule.Category);
    }

    [Fact]
    public void Derive_NoData_DefaultsToMedium()
    {
        var plant = PlantWith(GrowthInfo.Empty);

        var schedule = _calculator.Derive(plant);

        Assert.Equal(WateringCategory.Medium, schedule.Category);
        Assert.Equal(7, schedule.DaysBetweenWatering);
    }
}
