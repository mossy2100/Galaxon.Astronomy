using Galaxon.Astronomy.Repository;
using Galaxon.Core.Exceptions;
using Galaxon.Numerics.Geometry;
using Galaxon.Numerics.Maths;
using Galaxon.Quantities;
using GeoCoordinatePortable;

namespace Galaxon.Astronomy.Algorithms;

public class World
{
    /// <summary>
    /// Calculate shortest distance between two points along the surface of a
    /// oblate spheroid world using Andoyer's method.
    /// The formula is from Astronomical Algorithms 2nd ed. by Jeen Meeus, p85.
    /// Unlike the Haversine formula, which assumes a spherical body, Andoyer's
    /// method takes flattening into account.
    /// Although you can provide radiuses for other worlds, this algorithm was
    /// designed for Earth and therefore assumes:
    /// (a) the object is an oblate spheroid;
    /// (b) coordinates are given in degrees;
    /// (c) the usual coordinate system is used, i.e. latitude is in the range
    /// -90..90, and longitude is in the range -180..180
    /// Altitude is ignored.
    /// </summary>
    /// <param name="location1">The geographical coordinates of location 1.</param>
    /// <param name="location2">The geographical coordinates of location 2.</param>
    /// <param name="radiusEquat">The equatorial radius in kilometres.</param>
    /// <param name="radiusPolar">The polar radius in kilometres.</param>
    /// <returns>The distance between the two locations in kilometres.</returns>
    public static double ShortestDistanceBetween(GeoCoordinate location1,
        GeoCoordinate location2, double radiusEquat, double radiusPolar)
    {
        // Calculate the flattening.
        double f = (radiusEquat - radiusPolar) / radiusEquat;

        double F = Angle.DegToRad((location1.Latitude + location2.Latitude) / 2);
        double sin2F = Angle.Sin2(F);
        double cos2F = Angle.Cos2(F);

        double G = Angle.DegToRad((location1.Latitude - location2.Latitude) / 2);
        double sin2G = Angle.Sin2(G);
        double cos2G = Angle.Cos2(G);

        double lambda = Angle.DegToRad((location1.Longitude - location2.Longitude) / 2);
        double sin2Lambda = Angle.Sin2(lambda);
        double cos2Lambda = Angle.Cos2(lambda);

        double S = sin2G * cos2Lambda + cos2F * sin2Lambda;
        double C = cos2G * cos2Lambda + sin2F * sin2Lambda;

        double omega = Math.Atan(Math.Sqrt(S / C));
        double R = Math.Sqrt(S * C) / omega;
        double D = 2 * omega * radiusEquat;
        double H1 = (3 * R - 1) / 2 / C;
        double H2 = (3 * R + 1) / 2 / S;
        return D * (1 + f * H1 * sin2F * cos2G - f * H2 * cos2F * sin2G);
    }

    public static double ShortestDistanceBetween(GeoCoordinate location1,
        GeoCoordinate location2, Planet planet) =>
        ShortestDistanceBetween(location1, location2, planet.Physical!.EquatorialRadius,
            planet.Physical.PolarRadius);

    /// <summary>
    /// Calculate the position of a planet in heliocentric ecliptic coordinates.
    /// Algorithm is from AA2 p218.
    /// The result is a tuple with 3 coordinate values:
    ///   L, the heliocentric longitude in radians, in range -PI..PI
    ///   B, the heliocentric latitude in radians, in range -PI/2..PI/2
    ///   R, the orbital radius in metres.
    /// <see href="https://www.caglow.com/info/compute/vsop87"/>
    /// Original data files are from:
    /// <see href="ftp://ftp.imcce.fr/pub/ephem/planets/vsop87"/>
    /// </summary>
    /// <param name="planet">The planet.</param>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns>The planet's position in heliocentric ecliptic
    /// coordinates.</returns>
    /// <exception cref="DataNotFoundException">If no VSOP87D data could be
    /// found for the planet.</exception>
    public static (double L, double B, double R) CalcPlanetPosition(Planet planet, double jdtt)
    {
        // Get the VSOP87D data for the planet from the database.
        // These aren't included in Load() so I may need to get them separately
        // rather than via the VSOP87DRecords property.
        using AstroDbContext db = new();
        List<VSOP87DRecord>? records = db.VSOP87D
            .Where(r => r.AstroObjectId == planet.Id)
            .ToList();

        // Check there are records.
        if (records == null || records.Count == 0)
        {
            throw new DataNotFoundException($"No VSOP87D data found for planet {planet.Name}.");
        }

        // Get T in Julian millennia from the epoch J2000.0.
        double T = Terran.JulianMillenniaSinceJ2000(jdtt);

        // Calculate the coefficients for each coordinate variable.
        Dictionary<char, double[]> coeffs = new();
        foreach (VSOP87DRecord record in records)
        {
            if (!coeffs.ContainsKey(record.Variable))
            {
                coeffs[record.Variable] = new double[6];
            }
            double amplitude = record.Amplitude;
            double phase = record.Phase;
            double frequency = record.Frequency;
            coeffs[record.Variable][record.Exponent] += amplitude * Cos(phase + frequency * T);
        }

        // Calculate each coordinate variable.
        double L = Angle.NormalizeRadians(Equations.EvaluatePolynomial(coeffs['L'], T));
        double B = Angle.NormalizeRadians(Equations.EvaluatePolynomial(coeffs['B'], T));
        double R = Equations.EvaluatePolynomial(coeffs['R'], T) * Length.MetresPerAu;
        return (L, B, R);
    }
}
