namespace Galaxon.Astronomy.Repository;

public class Stellar
{
    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }
    public AstroObject? AstroObject { get; set; }

    // Spectral classification.
    public string? SpectralClass { get; set; }

    // Metallicity.
    public double? Metallicity { get; set; }

    // The luminosity (W).
    public double? Luminosity { get; set; }

    // Mean radiance (W/sr/m2).
    public double? Radiance { get; set; }
}
