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
    public const double DAYS_PER_SOL = 1.02749;

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
    /// <param name="JD">The Julian Date.</param>
    /// <returns>The Mars Sol Date.</returns>
    public static double CalcMarsSolDate(double JD)
    {
        var jd_tai = JulianDateService.JulianDate_UT_to_TT(JD);

        return (JD - 2405522.0) / DAYS_PER_SOL;
    }

    public (double L, double B, double R) CalcPosition(double JD_TT)
    {
        AstroObject mars = GetPlanet();
        return planetService.CalcPlanetPosition(mars, JD_TT);
    }
}
