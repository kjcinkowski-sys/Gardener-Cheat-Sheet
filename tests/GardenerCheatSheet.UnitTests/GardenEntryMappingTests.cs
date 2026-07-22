using GardenerCheatSheet.Application.Mapping;
using GardenerCheatSheet.Application.Services;
using GardenerCheatSheet.Domain.Garden;
using GardenerCheatSheet.Domain.Plants;
using Xunit;

namespace GardenerCheatSheet.UnitTests;

public class GardenEntryMappingTests
{
    private static readonly DateOnly Today = new(2026, 7, 22);
    private readonly WateringScheduleCalculator _calculator = new();

    [Fact]
    public void ToDto_CustomEntry_IsCustom_NullTrefleId_UsesPhotoOverride()
    {
        var plant = Plant.CreateCustom(
            "Habanero pepper", "Capsicum chinense", null, null, new GrowthInfo { Light = 8 }, isIndoor: false);
        var entry = new GardenEntry(plant, nickname: null, dateAdded: Today);
        entry.SetImageUrl("/uploads/plants/abc.jpg");

        var schedule = _calculator.Derive(plant);
        var dto = PlantMapper.ToDto(entry, schedule, Today);

        Assert.True(dto.IsCustom);
        Assert.Null(dto.TrefleId);
        Assert.Equal("Habanero pepper", dto.DisplayName);
        Assert.Equal("Capsicum chinense", dto.ScientificName);
        Assert.Equal("/uploads/plants/abc.jpg", dto.ImageUrl);
    }

    [Fact]
    public void ToDto_TrefleEntry_IsNotCustom_CarriesTrefleId()
    {
        var plant = new Plant(42, "Rosa", "Rose", null, "http://trefle/rose.jpg", GrowthInfo.Empty, false);
        var entry = new GardenEntry(plant, nickname: null, dateAdded: Today);

        var schedule = _calculator.Derive(plant);
        var dto = PlantMapper.ToDto(entry, schedule, Today);

        Assert.False(dto.IsCustom);
        Assert.Equal(42, dto.TrefleId);
        // With no override, the species image is used.
        Assert.Equal("http://trefle/rose.jpg", dto.ImageUrl);
    }
}
