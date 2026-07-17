using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Domain.Garden;

/// <summary>
/// The collection of plants a user is tending. In the single-user MVP there is
/// exactly one garden; when authentication lands this aggregate gains an owning
/// <c>UserId</c> and becomes the isolation boundary between users.
/// </summary>
public sealed class Garden
{
    private readonly List<GardenEntry> _entries;

    public Garden(IEnumerable<GardenEntry> entries)
    {
        _entries = entries.ToList();
    }

    public IReadOnlyList<GardenEntry> Entries => _entries;

    /// <summary>
    /// Entries whose next watering date is on or before <paramref name="asOf"/>,
    /// i.e. plants that are due (or overdue) for watering.
    /// </summary>
    /// <param name="scheduleFor">
    /// Resolves the species-derived schedule for a given entry. Supplied by the
    /// application layer, which owns the watering calculator.
    /// </param>
    public IEnumerable<GardenEntry> DueForWatering(
        DateOnly asOf,
        Func<GardenEntry, WateringSchedule> scheduleFor)
    {
        foreach (var entry in _entries)
        {
            var next = entry.NextWateringDate(scheduleFor(entry));
            if (next is DateOnly due && due <= asOf)
            {
                yield return entry;
            }
        }
    }
}
