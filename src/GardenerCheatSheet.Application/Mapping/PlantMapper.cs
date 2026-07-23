using GardenerCheatSheet.Application.Dtos;
using GardenerCheatSheet.Application.External;
using GardenerCheatSheet.Domain.Garden;
using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Application.Mapping;

/// <summary>
/// Translates between Trefle provider data, domain entities, and API DTOs.
/// </summary>
public static class PlantMapper
{
    /// <summary>
    /// Builds a domain <see cref="Plant"/> from provider data, choosing a
    /// default indoor/outdoor classification heuristically.
    /// </summary>
    public static Plant ToDomain(TreflePlantData data)
    {
        var growth = new GrowthInfo
        {
            Light = data.Light,
            AtmosphericHumidity = data.AtmosphericHumidity,
            MinimumPrecipitationMm = data.MinimumPrecipitationMm,
            MaximumPrecipitationMm = data.MaximumPrecipitationMm,
            SoilTexture = data.SoilTexture,
            Description = data.Description
        };

        return new Plant(
            trefleId: data.TrefleId,
            scientificName: data.ScientificName,
            commonName: data.CommonName,
            family: data.Family,
            imageUrl: data.ImageUrl,
            growth: growth,
            isIndoor: GuessIsIndoor(growth));
    }

    /// <summary>
    /// Heuristic default for indoor vs outdoor. Trefle has no such field, so we
    /// treat lower-light, higher-humidity plants as more likely houseplants.
    /// The user can override this per garden entry.
    /// </summary>
    public static bool GuessIsIndoor(GrowthInfo growth)
    {
        // Plants that tolerate shade and prefer humid air lean "indoor".
        var lowLight = growth.Light is int light && light <= 4;
        var humid = growth.AtmosphericHumidity is int h && h >= 5;
        return lowLight && humid;
    }

    public static PlantSummaryDto ToSummary(TreflePlantData data) => new()
    {
        TrefleId = data.TrefleId,
        DisplayName = string.IsNullOrWhiteSpace(data.CommonName) ? data.ScientificName : data.CommonName!,
        ScientificName = data.ScientificName,
        ImageUrl = data.ImageUrl,
        IsIndoor = GuessIsIndoor(new GrowthInfo { Light = data.Light, AtmosphericHumidity = data.AtmosphericHumidity })
    };

    public static PlantDetailDto ToDetail(Plant plant, WateringSchedule watering) => new()
    {
        TrefleId = plant.TrefleId ?? 0,
        DisplayName = plant.DisplayName,
        ScientificName = plant.ScientificName,
        CommonName = plant.CommonName,
        Family = plant.Family,
        ImageUrl = plant.ImageUrl,
        Description = plant.Growth.Description,
        SoilTexture = plant.Growth.SoilTexture,
        LightRequirement = plant.LightRequirement.ToDisplayString(),
        IsIndoor = plant.IsIndoor,
        Watering = ToWateringDto(watering)
    };

    public static GardenEntryDto ToDto(GardenEntry entry, WateringSchedule resolved, DateOnly today)
    {
        var next = entry.NextWateringDate(resolved);
        return new GardenEntryDto
        {
            Id = entry.Id,
            TrefleId = entry.Plant.TrefleId,
            IsCustom = entry.Plant.Source == PlantSource.Custom,
            DisplayName = string.IsNullOrWhiteSpace(entry.Nickname) ? entry.Plant.DisplayName : entry.Nickname!,
            PlantName = entry.Plant.DisplayName,
            Nickname = entry.Nickname,
            ScientificName = entry.Plant.ScientificName,
            ImageUrl = entry.ImageUrl,
            Notes = entry.Notes,
            IsIndoor = entry.IsIndoor,
            LightRequirement = entry.Plant.LightRequirement.ToDisplayString(),
            Watering = ToWateringDto(resolved),
            DateAdded = entry.DateAdded,
            LastWatered = entry.LastWatered,
            NextWateringDate = next,
            IsDue = next is DateOnly due && due <= today
        };
    }

    public static WateringScheduleDto ToWateringDto(WateringSchedule schedule) => new()
    {
        Category = schedule.Category.ToString(),
        DaysBetweenWatering = schedule.DaysBetweenWatering,
        Source = schedule.Source.ToString()
    };
}
