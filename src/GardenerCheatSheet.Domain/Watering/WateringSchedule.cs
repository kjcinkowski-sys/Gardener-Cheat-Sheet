namespace GardenerCheatSheet.Domain.Watering;

/// <summary>
/// A value object describing how often a plant should be watered.
/// Immutable: create a new instance to change it.
/// </summary>
public sealed record WateringSchedule
{
    public WateringCategory Category { get; }

    /// <summary>Number of days between waterings.</summary>
    public int DaysBetweenWatering { get; }

    public WateringSource Source { get; }

    public WateringSchedule(WateringCategory category, int daysBetweenWatering, WateringSource source)
    {
        if (daysBetweenWatering < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(daysBetweenWatering),
                "A watering schedule must have at least one day between waterings.");
        }

        Category = category;
        DaysBetweenWatering = daysBetweenWatering;
        Source = source;
    }

    /// <summary>
    /// Returns a copy of this schedule marked as a user override with the
    /// supplied cadence. The category is inferred from the cadence.
    /// </summary>
    public WateringSchedule WithUserOverride(int daysBetweenWatering) =>
        new(CategoryForCadence(daysBetweenWatering), daysBetweenWatering, WateringSource.UserOverride);

    /// <summary>
    /// Given a plant's last-watered date, returns the next date it is due.
    /// </summary>
    public DateOnly NextWateringDate(DateOnly lastWatered) =>
        lastWatered.AddDays(DaysBetweenWatering);

    private static WateringCategory CategoryForCadence(int days) => days switch
    {
        >= 10 => WateringCategory.Low,
        >= 4 => WateringCategory.Medium,
        _ => WateringCategory.High
    };
}
