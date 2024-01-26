using System.Data;
using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Models;
using Galaxon.Core.Numbers;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// This is a static class containing useful methods and constants relating to
/// time.
///
/// This includes methods and constants for converting between time scales,
/// including:
/// - Terrestrial Time (TT)
/// - Universal Time (UT1)
/// - Coordinated Universal Time (UTC)
/// - International Atomic Time (TAI)
/// Differences are calculated in SI seconds in all cases.
/// </summary>
public class EarthService
{
    private AstroObjectRepository _repo;

    private AstroObject? _earth;

    public EarthService(AstroObjectRepository repo)
    {
        _repo = repo;
    }

    public AstroObject GetPlanet()
    {
        if (_earth == null)
        {
            // TODO make this call async
            _earth = _repo.Load("Earth");
        }
        return _earth;
    }

    /// <summary>
    /// Calculate the Earth Rotation Angle from the Julian Date in UT1.
    /// <see href="https://en.wikipedia.org/wiki/Sidereal_time#ERA"/>
    /// TODO This method probably belongs in a different class.
    /// </summary>
    /// <param name="jdut">The Julian Date in UT1.</param>
    /// <returns>The Earth Rotation Angle.</returns>
    public static double CalcERA(double jdut)
    {
        double t = TimeScaleService.JulianDaysSinceJ2000(jdut);
        double radians = Tau * (0.779_057_273_264 + 1.002_737_811_911_354_48 * t);
        return Angle.NormalizeRadians(radians);
    }

    /// <summary>
    /// Overload of CalcERA() that accepts a UTC DateTime.
    /// </summary>
    /// <param name="dtut">The instant.</param>
    /// <returns>The ERA at the given instant.</returns>
    public static double CalcERA(DateTime dtut)
    {
        return CalcERA(dtut.ToJulianDate());
    }

    public (double L, double B, double R) CalcPosition(double jdtt)
    {
        AstroObject earth = GetPlanet();
        return PlanetService.CalcPlanetPosition(earth, jdtt);
    }
}
