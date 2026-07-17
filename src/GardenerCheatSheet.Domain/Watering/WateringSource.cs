namespace GardenerCheatSheet.Domain.Watering;

/// <summary>
/// Where a <see cref="WateringSchedule"/> came from: derived automatically
/// from Trefle growth data, or set explicitly by the user.
/// </summary>
public enum WateringSource
{
    Derived = 0,
    UserOverride
}
