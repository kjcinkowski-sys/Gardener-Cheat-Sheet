using GardenerCheatSheet.Domain.Watering;

namespace GardenerCheatSheet.Domain.Plants;

/// <summary>
/// A plant species. Sourced from Trefle and cached locally so a garden entry
/// can reference stable plant data without hitting the external API every time.
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
        ScientificName = scientificName;
        CommonName = commonName;
        Family = family;
        ImageUrl = imageUrl;
        Growth = growth;
        IsIndoor = isIndoor;
    }

    /// <summary>Local surrogate key.</summary>
    public int Id { get; private set; }

    /// <summary>The plant's identifier in Trefle (its natural key).</summary>
    public int TrefleId { get; private set; }

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
}
