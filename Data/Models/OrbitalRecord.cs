namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// Represents an orbital record associated with an astronomical object.
/// </summary>
public class OrbitalRecord
{
    /// <summary>
    /// Gets or sets the primary key of the orbital record.
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
    /// Gets or sets the date/time of reference for the current orbital parameters.
    /// </summary>
    public DateTime? Epoch { get; set; }

    /// <summary>
    /// Gets or sets the eccentricity of the orbit.
    /// </summary>
    public double? Eccentricity { get; set; }

    /// <summary>
    /// Gets or sets the semi-major axis of the orbit in kilometers.
    /// </summary>
    public double? SemiMajorAxis { get; set; }

    /// <summary>
    /// Gets or sets the inclination to the ecliptic in degrees.
    /// </summary>
    public double? Inclination { get; set; }

    /// <summary>
    /// Gets or sets the longitude of the ascending node in degrees.
    /// </summary>
    public double? LongAscNode { get; set; }

    /// <summary>
    /// Gets or sets the argument of periapsis in degrees.
    /// </summary>
    public double? ArgPeriapsis { get; set; }

    /// <summary>
    /// Gets or sets the mean anomaly in degrees.
    /// </summary>
    public double? MeanAnomaly { get; set; }

    /// <summary>
    /// Gets or sets the apoapsis/aphelion of the orbit in kilometers.
    /// </summary>
    public double? Apoapsis { get; set; }

    /// <summary>
    /// Gets or sets the periapsis/perihelion of the orbit in kilometers.
    /// </summary>
    public double? Periapsis { get; set; }

    /// <summary>
    /// Gets or sets the sidereal orbit period in days.
    /// </summary>
    public double? SiderealOrbitPeriod { get; set; }

    /// <summary>
    /// Gets or sets the synodic orbit period in days.
    /// </summary>
    public double? SynodicOrbitPeriod { get; set; }

    /// <summary>
    /// Gets or sets the average orbital speed in kilometers per second.
    /// </summary>
    public double? AvgOrbitSpeed { get; set; }

    /// <summary>
    /// Gets or sets the mean daily motion in degrees per day.
    /// </summary>
    public double? MeanMotion { get; set; }

    /// <summary>
    /// Gets the longitude of the periapsis in degrees.
    /// </summary>
    /// <see href="https://en.wikipedia.org/wiki/Longitude_of_the_periapsis"/>
    public double? LongPeriapsis => LongAscNode + ArgPeriapsis;

    /// <summary>
    /// Calculates the approximate true anomaly in degrees.
    /// </summary>
    /// <see href="https://en.wikipedia.org/wiki/True_anomaly#From_the_mean_anomaly"/>
    /// <remarks>
    /// Note that for reasons of accuracy, this approximation is usually limited to orbits
    /// where the eccentricity (e) is small.
    /// </remarks>
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
            return M + (2 * e - e3 / 4) * Sin(M) + 5 * e * e * Sin(2 * M) / 4
                + 13 * e3 * Sin(3 * M) / 12;
        }
    }
}
