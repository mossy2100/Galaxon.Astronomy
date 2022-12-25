namespace Galaxon.Astronomy.Repository;

public class Orbit
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }
    public AstroObject? AstroObject { get; set; }

    // The date/time of reference for the current orbital parameters.
    public DateTime? Epoch { get; set; }

    // Eccentricity.
    public double? Eccentricity { get; set; }

    // Semi-major axis (km).
    public double? SemiMajorAxis { get; set; }

    // Inclination to ecliptic (°).
    public double? Inclination { get; set; }

    // Longitude of ascending node (°).
    public double? LongAscNode { get; set; }

    // Argument of periapsis (°).
    public double? ArgPeriapsis { get; set; }

    // Mean anomaly (°).
    public double? MeanAnomaly { get; set; }

    // Apoapsis/aphelion (km).
    public double? Apoapsis { get; set; }

    // Periapsis/perihelion (km).
    public double? Periapsis { get; set; }

    // Sidereal orbit period (days).
    public double? SiderealOrbitPeriod { get; set; }

    // Synodic orbit period (days).
    public double? SynodicOrbitPeriod { get; set; }

    // Average orbital speed (km/s).
    public double? AvgOrbitSpeed { get; set; }

    // Mean daily motion (°/d).
    public double? MeanMotion { get; set; }

    #endregion Properties

    /// <summary>
    /// Get the longitude of the periapsis (°).
    /// <see href="https://en.wikipedia.org/wiki/Longitude_of_the_periapsis"/>
    /// </summary>
    public double? LongPeriapsis => LongAscNode + ArgPeriapsis;

    /// <summary>
    /// Calculate the approximate true anomaly.
    /// <see href="https://en.wikipedia.org/wiki/True_anomaly#From_the_mean_anomaly"/>
    /// "Note that for reasons of accuracy this approximation is usually limited
    /// to orbits where the eccentricity (e) is small."
    /// </summary>
    public double? ApproxTrueAnomaly
    {
        get
        {
            if (MeanAnomaly == null || Eccentricity == null)
            {
                return null;
            }
            double M = MeanAnomaly.Value;
            double e = Eccentricity.Value;
            double e3 = Pow(e, 3);
            return M + (2 * e - e3 / 4) * Sin(M) +
                5 * e * e * Sin(2 * M) / 4 +
                13 * e3 * Sin(3 * M) / 12;
        }
    }
}
