namespace GardenerCheatSheet.Domain.Watering;

/// <summary>
/// How thirsty a plant is. Drives the default number of days between watering.
/// </summary>
public enum WateringCategory
{
    Unknown = 0,

    /// <summary>Drought tolerant — water infrequently (e.g. succulents).</summary>
    Low,

    /// <summary>Average watering needs.</summary>
    Medium,

    /// <summary>Thirsty — needs frequent watering.</summary>
    High
}
