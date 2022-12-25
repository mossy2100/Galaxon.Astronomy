namespace Galaxon.Astronomy.Repository;

public class Rotation
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }
    public AstroObject? AstroObject { get; set; }

    // Sidereal rotation period in days.
    public double? SiderealRotationPeriod { get; set; }

    // Synodic rotation period in days.
    public double? SynodicRotationPeriod { get; set; }

    // Equatorial rotational velocity in km/s.
    public double? EquatRotationVelocity { get; set; }

    // Axial tilt (obliquity) in degrees.
    public double? Obliquity { get; set; }

    // North pole right ascension in degrees.
    public double? NorthPoleRightAscension { get; set; }

    // North pole declination in degrees.
    public double? NorthPoleDeclination { get; set; }

    #endregion Properties

    public Rotation()
    {
    }
}
