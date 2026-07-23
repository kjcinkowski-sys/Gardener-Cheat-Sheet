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
        Assert.Equal("Habanero pepper", dto.PlantName);
        Assert.Equal("Capsicum chinense", dto.ScientificName);
        Assert.Equal("/uploads/plants/abc.jpg", dto.ImageUrl);
    }

    [Fact]
    public void ToDto_WithNickname_DisplayNameIsNickname_ButPlantNameIsSpecies()
    {
        var plant = Plant.CreateCustom("Habanero pepper", null, null, null, GrowthInfo.Empty, isIndoor: false);
        var entry = new GardenEntry(plant, nickname: "Spicy Steve", dateAdded: Today);

        var dto = PlantMapper.ToDto(entry, _calculator.Derive(plant), Today);

        // The heading shows the nickname, but the plant's own name is preserved for editing.
        Assert.Equal("Spicy Steve", dto.DisplayName);
        Assert.Equal("Habanero pepper", dto.PlantName);
        Assert.Equal("Spicy Steve", dto.Nickname);
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
