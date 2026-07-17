using GardenerCheatSheet.Domain.Watering;
using Xunit;

namespace GardenerCheatSheet.UnitTests;

public class WateringScheduleTests
{
    [Fact]
    public void NextWateringDate_AddsCadenceToLastWatered()
    {
        var schedule = new WateringSchedule(WateringCategory.Medium, 7, WateringSource.Derived);
        var lastWatered = new DateOnly(2026, 7, 1);

        Assert.Equal(new DateOnly(2026, 7, 8), schedule.NextWateringDate(lastWatered));
    }

    [Fact]
    public void WithUserOverride_MarksSourceAndInfersCategory()
    {
        var derived = new WateringSchedule(WateringCategory.Medium, 7, WateringSource.Derived);

        var overridden = derived.WithUserOverride(14);

        Assert.Equal(WateringSource.UserOverride, overridden.Source);
        Assert.Equal(14, overridden.DaysBetweenWatering);
        Assert.Equal(WateringCategory.Low, overridden.Category);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Constructor_RejectsNonPositiveCadence(int days)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WateringSchedule(WateringCategory.Medium, days, WateringSource.Derived));
    }
}
