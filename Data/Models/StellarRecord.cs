namespace Galaxon.Astronomy.Models;

/// <summary>
/// Represents information about a stellar object.
/// </summary>
public class StellarRecord
{
    /// <summary>
    /// Gets or sets the primary key of the stellar record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the link to the astronomical object associated with this record.
    /// </summary>
    public int AstroObjectId { get; set; }

    /// <summary>
    /// Gets or sets the reference to the astronomical object associated with this record.
    /// </summary>
    public AstroObject? AstroObject { get; set; }

    /// <summary>
    /// Gets or sets the spectral classification of the stellar object.
    /// </summary>
    public string? SpectralClass { get; set; }

    /// <summary>
    /// Gets or sets the metallicity of the stellar object.
    /// </summary>
    public double? Metallicity { get; set; }

    /// <summary>
    /// Gets or sets the luminosity of the stellar object in watts (W).
    /// </summary>
    public double? Luminosity { get; set; }

    /// <summary>
    /// Gets or sets the mean radiance of the stellar object in watts per steradian per square meter (W/sr/m²).
    /// </summary>
    public double? Radiance { get; set; }
}
