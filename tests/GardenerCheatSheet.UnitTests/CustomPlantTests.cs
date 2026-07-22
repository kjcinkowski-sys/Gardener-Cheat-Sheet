using GardenerCheatSheet.Domain.Garden;
using GardenerCheatSheet.Domain.Plants;
using Xunit;

namespace GardenerCheatSheet.UnitTests;

public class CustomPlantTests
{
    private static readonly DateOnly Today = new(2026, 7, 22);

    [Fact]
    public void CreateCustom_SetsCustomSource_AndNoTrefleId()
    {
        var plant = Plant.CreateCustom(
            displayName: "Habanero pepper",
            scientificName: null,
            family: null,
            imageUrl: null,
            growth: GrowthInfo.Empty,
            isIndoor: false);

        Assert.Equal(PlantSource.Custom, plant.Source);
        Assert.Null(plant.TrefleId);
        Assert.Equal("Habanero pepper", plant.DisplayName);
        // Scientific name falls back to the display name when omitted.
        Assert.Equal("Habanero pepper", plant.ScientificName);
    }

    [Fact]
    public void CreateCustom_BlankName_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            Plant.CreateCustom("  ", null, null, null, GrowthInfo.Empty, false));
    }

    [Fact]
    public void EditCustomDetails_UpdatesIdentityAndLight()
    {
        var plant = Plant.CreateCustom("Habanero", null, null, null, GrowthInfo.Empty, false);

        plant.EditCustomDetails("Ghost pepper", "Capsicum chinense", new GrowthInfo { Light = 8 });

        Assert.Equal("Ghost pepper", plant.DisplayName);
        Assert.Equal("Capsicum chinense", plant.ScientificName);
        Assert.Equal(8, plant.Growth.Light);
    }

    [Fact]
    public void EditCustomDetails_OnTreflePlant_Throws()
    {
        var trefle = new Plant(1, "Rosa", "Rose", null, null, GrowthInfo.Empty, isIndoor: false);

        Assert.Throws<InvalidOperationException>(() =>
            trefle.EditCustomDetails("Nope", null, GrowthInfo.Empty));
    }

    [Fact]
    public void EntryImageUrl_PrefersOverride_ThenFallsBackToPlant()
    {
        var plant = new Plant(1, "Rosa", "Rose", null, "http://trefle/rose.jpg", GrowthInfo.Empty, false);
        var entry = new GardenEntry(plant, nickname: null, dateAdded: Today);

        // No override yet -> species image.
        Assert.Equal("http://trefle/rose.jpg", entry.ImageUrl);

        entry.SetImageUrl("/uploads/plants/myrose.jpg");
        Assert.Equal("/uploads/plants/myrose.jpg", entry.ImageUrl);

        // Blank clears the override and falls back again.
        entry.SetImageUrl("  ");
        Assert.Equal("http://trefle/rose.jpg", entry.ImageUrl);
    }
}
