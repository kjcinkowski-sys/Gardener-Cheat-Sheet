using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Domain.Plants;

/// <summary>
/// A plant species. Usually sourced from Trefle and cached locally so a garden
/// entry can reference stable plant data without hitting the external API every
/// time. May also be a user-created custom plant (see <see cref="PlantSource"/>)
/// for species Trefle does not list.
/// </summary>
public class Plant
{
    // Parameterless constructor for EF Core materialisation.
    private Plant()
    {
        ScientificName = string.Empty;
        Growth = new GrowthInfo();
    }

    public Plant(
        int trefleId,
        string scientificName,
        string? commonName,
        string? family,
        string? imageUrl,
        GrowthInfo growth,
        bool isIndoor)
    {
        if (string.IsNullOrWhiteSpace(scientificName))
        {
            throw new ArgumentException("A plant must have a scientific name.", nameof(scientificName));
        }

        TrefleId = trefleId;
        Source = PlantSource.Trefle;
        ScientificName = scientificName;
        CommonName = commonName;
        Family = family;
        ImageUrl = imageUrl;
        Growth = growth;
        IsIndoor = isIndoor;
    }

    /// <summary>
    /// Creates a user-defined plant that has no Trefle counterpart. The display
    /// name is required; a scientific name is optional and falls back to the
    /// display name so <see cref="DisplayName"/> and <see cref="ScientificName"/>
    /// are always populated.
    /// </summary>
    public static Plant CreateCustom(
        string displayName,
        string? scientificName,
        string? family,
        string? imageUrl,
        GrowthInfo growth,
        bool isIndoor)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A custom plant must have a name.", nameof(displayName));
        }

        return new Plant
        {
            TrefleId = null,
            Source = PlantSource.Custom,
            CommonName = displayName.Trim(),
            ScientificName = string.IsNullOrWhiteSpace(scientificName)
                ? displayName.Trim()
                : scientificName.Trim(),
            Family = family,
            ImageUrl = imageUrl,
            Growth = growth,
            IsIndoor = isIndoor
        };
    }

    /// <summary>Local surrogate key.</summary>
    public int Id { get; private set; }

    /// <summary>
    /// The plant's identifier in Trefle (its natural key). Null for custom,
    /// user-created plants.
    /// </summary>
    public int? TrefleId { get; private set; }

    /// <summary>Whether this plant came from Trefle or was created by the user.</summary>
    public PlantSource Source { get; private set; }

    public string ScientificName { get; private set; }

    public string? CommonName { get; private set; }

    public string? Family { get; private set; }

    public string? ImageUrl { get; private set; }

    public GrowthInfo Growth { get; private set; }

    /// <summary>
    /// Default indoor/outdoor classification for the species. Trefle has no
    /// direct field for this, so it is a heuristic default that a user can
    /// override on their own <see cref="Garden.GardenEntry"/>.
    /// </summary>
    public bool IsIndoor { get; private set; }

    public LightRequirement LightRequirement => LightRequirementExtensions.FromTrefleLight(Growth.Light);

    /// <summary>A friendly display name, preferring the common name.</summary>
    public string DisplayName =>
        string.IsNullOrWhiteSpace(CommonName) ? ScientificName : CommonName!;

    /// <summary>
    /// Refreshes cached data from a newer Trefle read while keeping the same
    /// local identity.
    /// </summary>
    public void UpdateFromSource(
        string scientificName,
        string? commonName,
        string? family,
        string? imageUrl,
        GrowthInfo growth,
        bool isIndoor)
    {
        ScientificName = scientificName;
        CommonName = commonName;
        Family = family;
        ImageUrl = imageUrl;
        Growth = growth;
        IsIndoor = isIndoor;
    }

    /// <summary>
    /// Edits the identity and light fields of a custom plant. Only valid for
    /// <see cref="PlantSource.Custom"/> plants; Trefle plants are refreshed via
    /// <see cref="UpdateFromSource"/> and must not be hand-edited. Indoor/outdoor
    /// and watering cadence remain per-entry overrides on
    /// <see cref="Garden.GardenEntry"/>, so they are not set here.
    /// </summary>
    public void EditCustomDetails(
        string displayName,
        string? scientificName,
        GrowthInfo growth)
    {
        if (Source != PlantSource.Custom)
        {
            throw new InvalidOperationException("Only custom plants can be edited directly.");
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("A custom plant must have a name.", nameof(displayName));
        }

        CommonName = displayName.Trim();
        ScientificName = string.IsNullOrWhiteSpace(scientificName)
            ? displayName.Trim()
            : scientificName.Trim();
        Growth = growth;
    }
}
