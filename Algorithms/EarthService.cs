using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// This is a static class containing useful methods and constants relating to Earth.
/// </summary>
public class EarthService(AstroObjectRepository astroObjectRepository, PlanetService planetService)
{
    /// <summary>
    /// Cached reference to the AstroObject representing Earth.
    /// </summary>
    private AstroObject? _earth;

    /// <summary>
    /// Get the AstroObject representing Earth.
    /// </summary>
    /// <exception cref="DataNotFoundException">
    /// If the object could not be loaded from the database.
    /// </exception>
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
    /// </summary>
    /// <param name="JD">The Julian Date in UT1.</param>
    /// <returns>The Earth Rotation Angle.</returns>
    public static double CalcEarthRotationAngle(double JD)
    {
        double t = JulianDateService.JulianDaysSinceJ2000(JD);
        double radians = Tau * (0.779_057_273_264 + 1.002_737_811_911_354_48 * t);
        return Angle.NormalizeRadians(radians);
    }

    /// <summary>
    /// Calculate the Earth Rotation Angle from a UTC DateTime.
    /// </summary>
    /// <param name="dt">The instant.</param>
    /// <returns>The ERA at the given instant.</returns>
    public static double CalcEarthRotationAngle(DateTime dt)
    {
        double JD = JulianDateService.DateTime_to_JulianDate(dt);
        return CalcEarthRotationAngle(JD);
    }

    /// <summary>
    /// Calculate the heliocentric position of Earth at a given point in time.
    /// </summary>
    /// <param name="JD_TT">The Julian Date in Terrestrial Time.</param>
    /// <returns>Heliocentric coordinates of Earth.</returns>
    public (double L, double B, double R) CalcPosition(double JD_TT)
    {
        AstroObject earth = GetPlanet();
        return planetService.CalcPlanetPosition(earth, JD_TT);
    }
}
