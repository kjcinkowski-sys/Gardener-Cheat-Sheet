namespace GardenerCheatSheet.Domain.Plants;

/// <summary>
/// A coarse light-need category. Derived from Trefle's <c>growth.light</c>
/// value, which is reported on a 0–10 scale.
/// </summary>
public enum LightRequirement
{
    Unknown = 0,
    Shade,
    PartialShade,
    FullSun
}

public static class LightRequirementExtensions
{
    /// <summary>
    /// Maps Trefle's 0–10 light scale to a <see cref="LightRequirement"/>.
    /// Roughly: 0–3 shade, 4–6 partial shade, 7–10 full sun.
    /// </summary>
    public static LightRequirement FromTrefleLight(int? light)
    {
        if (light is null)
        {
            return LightRequirement.Unknown;
        }

        return light switch
        {
            <= 3 => LightRequirement.Shade,
            <= 6 => LightRequirement.PartialShade,
            _ => LightRequirement.FullSun
        };
    }

    public static string ToDisplayString(this LightRequirement light) => light switch
    {
        LightRequirement.Shade => "Shade",
        LightRequirement.PartialShade => "Partial shade",
        LightRequirement.FullSun => "Full sun",
        _ => "Unknown"
    };
}
