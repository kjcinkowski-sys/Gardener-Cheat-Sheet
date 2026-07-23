using GardenerCheatSheet.Domain.Plants;
using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Domain.Garden;

/// <summary>
/// A single plant a user has added to their garden. Holds the user's own
/// tracking data (nickname, notes, last watered) plus optional overrides of
/// the species defaults (watering cadence, indoor/outdoor).
/// </summary>
public class GardenEntry
{
    // Parameterless constructor for EF Core materialisation.
    private GardenEntry() { }

    public GardenEntry(Plant plant, string? nickname, DateOnly dateAdded)
    {
        Plant = plant ?? throw new ArgumentNullException(nameof(plant));
        PlantId = plant.Id;
        Nickname = nickname;
        DateAdded = dateAdded;
    }

    public int Id { get; private set; }

    public int PlantId { get; private set; }

    public Plant Plant { get; private set; } = null!;

    public string? Nickname { get; private set; }

    public DateOnly DateAdded { get; private set; }

    public DateOnly? LastWatered { get; private set; }

    public string? Notes { get; private set; }

    /// <summary>
    /// The user's own photo for this entry. Null means "use the species image"
    /// (<see cref="Plant.ImageUrl"/>). Lets a user set a personal photo on any
    /// entry — including a Trefle one — without mutating the shared species row.
    /// </summary>
    public string? ImageUrlOverride { get; private set; }

    /// <summary>Effective image, resolving the override.</summary>
    public string? ImageUrl => string.IsNullOrWhiteSpace(ImageUrlOverride) ? Plant.ImageUrl : ImageUrlOverride;

    /// <summary>
    /// Days between watering when the user has overridden the derived schedule.
    /// Null means "use the species-derived schedule".
    /// </summary>
    public int? WateringOverrideDays { get; private set; }

    /// <summary>
    /// Indoor/outdoor override. Null means "use the species default"
    /// (<see cref="Plant.IsIndoor"/>).
    /// </summary>
    public bool? IsIndoorOverride { get; private set; }

    /// <summary>Effective indoor/outdoor, resolving the override.</summary>
    public bool IsIndoor => IsIndoorOverride ?? Plant.IsIndoor;

    /// <summary>
    /// Resolves the effective watering schedule for this entry, applying any
    /// user override on top of the supplied species-derived schedule.
    /// </summary>
    public WateringSchedule ResolveSchedule(WateringSchedule derived) =>
        WateringOverrideDays is int days ? derived.WithUserOverride(days) : derived;

    /// <summary>
    /// The date this plant is next due for watering, or null if it has never
    /// been watered yet.
    /// </summary>
    public DateOnly? NextWateringDate(WateringSchedule derived) =>
        LastWatered is DateOnly last ? ResolveSchedule(derived).NextWateringDate(last) : null;

    public void Rename(string? nickname) => Nickname = nickname;

    public void SetNotes(string? notes) => Notes = notes;

    /// <summary>Sets or clears (null/empty) the user's own photo for this entry.</summary>
    public void SetImageUrl(string? imageUrl) =>
        ImageUrlOverride = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl;

    public void MarkWatered(DateOnly on) => LastWatered = on;

    public void OverrideWatering(int? daysBetweenWatering)
    {
        if (daysBetweenWatering is int days && days < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(daysBetweenWatering),
                "Watering cadence must be at least one day.");
        }

        WateringOverrideDays = daysBetweenWatering;
    }

    public void OverrideIndoor(bool? isIndoor) => IsIndoorOverride = isIndoor;
}
