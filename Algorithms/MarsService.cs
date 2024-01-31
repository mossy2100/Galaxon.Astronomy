using System.Runtime.InteropServices.ObjectiveC;
using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Exceptions;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// A container for constants and static methods related to Mars.
/// </summary>
public class MarsService(AstroObjectRepository astroObjectRepository, PlanetService planetService)
{
    /// <summary>
    /// Number of days (Earth solar days) per sol (Mars solar day).
    /// </summary>
    public const decimal DAYS_PER_SOL = 1.02749125M;

    /// <summary>
    /// Cached reference to the AstroObject representing Mars.
    /// </summary>
    private AstroObject? _mars;

    /// <summary>
    /// Get the AstroObject representing Mars.
    /// </summary>
    /// <exception cref="DataNotFoundException">
    /// If the object could not be loaded from the database.
    /// </exception>
    public AstroObject GetPlanet()
    {
        if (_mars == null)
        {
            AstroObject? mars = astroObjectRepository.Load("Mars", "planet");
            _mars = mars
                ?? throw new DataNotFoundException("Could not find planet Mars in the database.");
        }

        return _mars;
    }

    /// <summary>
    /// Calculate the Mars Sol Date for a given point in time, expressed as a Julian Date.
    /// </summary>
    /// <see href="https://en.wikipedia.org/wiki/Timekeeping_on_Mars#Mars_Sol_Date"/>
    /// <param name="JD_TT">The Julian Date (TT).</param>
    /// <returns>The Mars Sol Date.</returns>
    public static double CalcMarsSolDate(double JD_TT)
    {
        double JD_TAI = JulianDateService.JulianDate_TT_to_TAI(JD_TT);
        const double k = 1.0 / 4000;
        double MSD = (JD_TAI - 2451549.5 + k) / (double)DAYS_PER_SOL + 44796.0;
        return MSD;
    }

    /// <summary>
    /// Calculation position of Mars in heliocentric coordinates (radians).
    /// </summary>
    /// <param name="JD_TT">The Julian Date (TT).</param>
    /// <returns></returns>
    public (double L, double B, double R) CalcPosition(double JD_TT)
    {
        AstroObject mars = GetPlanet();
        return planetService.CalcPlanetPosition(mars, JD_TT);
    }
}
