using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// This is a static class containing useful methods and constants relating to Earth.
/// </summary>
public class EarthService(AstroObjectRepository astroObjectRepository, PlanetService planetService)
{
    private AstroObject? _earth;

    public AstroObject GetPlanet()
    {
        if (_earth == null)
        {
            AstroObject? earth = astroObjectRepository.Load("Earth", "planet");
            _earth = earth
                ?? throw new DataNotFoundException("Could not find planet Earth in the database.");
        }

        return _earth;
    }

    /// <summary>
    /// Calculate the Earth Rotation Angle from the Julian Date in UT1.
    /// <see href="https://en.wikipedia.org/wiki/Sidereal_time#ERA"/>
    /// TODO This method probably belongs in a different class.
    /// </summary>
    /// <param name="jd">The Julian Date in UT1.</param>
    /// <returns>The Earth Rotation Angle.</returns>
    public static double CalcEarthRotationAngle(double jd)
    {
        double t = TimeScaleService.JulianDaysSinceJ2000(jd);
        double radians = Tau * (0.779_057_273_264 + 1.002_737_811_911_354_48 * t);
        return Angle.NormalizeRadians(radians);
    }

    /// <summary>
    /// Overload of CalcERA() that accepts a UTC DateTime.
    /// </summary>
    /// <param name="dt">The instant.</param>
    /// <returns>The ERA at the given instant.</returns>
    public static double CalcEarthRotationAngle(DateTime dt)
    {
        return CalcEarthRotationAngle(dt.ToJulianDate());
    }

    /// <summary>
    /// Calculate the heliocentric position of Earth at a given point in time.
    /// </summary>
    /// <param name="jdtt">The Julian Date in Terrestrial Time.</param>
    /// <returns>Heliocentric coordinates of Earth.</returns>
    public (double L, double B, double R) CalcPosition(double jdtt)
    {
        AstroObject earth = GetPlanet();
        return planetService.CalcPlanetPosition(earth, jdtt);
    }
}
