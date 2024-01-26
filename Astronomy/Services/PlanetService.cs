using Galaxon.Astronomy.Database;
using Galaxon.Astronomy.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Numerics.Algebra;

namespace Galaxon.Astronomy.Services;

public class PlanetService
{
    /// <summary>
    /// Get a planet name given a number.
    /// </summary>
    /// <param name="num">The planet number as used in VSOP87 data.</param>
    /// <returns>The planet name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If the number is not in the range 1..8.
    /// </exception>
    public static string PlanetNumberToName(byte num)
    {
        return num switch
        {
            1 => "Mercury",
            2 => "Venus",
            3 => "Earth",
            4 => "Mars",
            5 => "Jupiter",
            6 => "Saturn",
            7 => "Uranus",
            8 => "Neptune",
            _ => throw new MatchNotFoundException("Planet number must be in the range 1..8.")
        };
    }

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
    public static (double L, double B, double R) CalcPlanetPosition(AstroObject planet, double jdtt)
    {
        // Get the VSOP87D data for the planet from the database.
        // These aren't included in Load() so I may need to get them separately
        // rather than via the VSOP87DRecords property.
        using AstroDbContext db = new ();
        var records = db.VSOP87DRecords
            .Where(r => r.AstroObjectId == planet.Id)
            .ToList();

        // Check there are records.
        if (records == null || records.Count == 0)
        {
            throw new DataNotFoundException($"No VSOP87D data found for planet {planet.Name}.");
        }

        // Get T in Julian millennia from the epoch J2000.0.
        double T = TimeScaleService.JulianMillenniaSinceJ2000(jdtt);

        // Calculate the coefficients for each coordinate variable.
        Dictionary<char, double[]> coeffs = new ();
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
        double L = Angle.NormalizeRadians(Polynomials.EvaluatePolynomial(coeffs['L'], T));
        double B = Angle.NormalizeRadians(Polynomials.EvaluatePolynomial(coeffs['B'], T));
        double R = Polynomials.EvaluatePolynomial(coeffs['R'], T) * Length.MetresPerAu;
        return (L, B, R);
    }
}
