using Galaxon.Astronomy.Repository;
using Galaxon.Core.Exceptions;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// A container for constants and static methods related to Mars.
/// </summary>
public static class Martian
{
    /// <summary>
    /// Number of days (Earth solar days) per sol (Mars solar day).
    /// </summary>
    public const double DAYS_PER_SOL = 1.02749;

    public static double CalcMarsSolDate(double jd)
    {
        return (jd - 2405522.0) / DAYS_PER_SOL;
    }

    public static Planet? GetPlanet()
    {
        using AstroDbContext db = new ();
        return Planet.Load(db, "Mars");
    }

    public static (double L, double B, double R) CalcPosition(double jdtt)
    {
        Planet? mars = GetPlanet();
        if (mars == null)
        {
            throw new DataNotFoundException("Could not find Mars in the database.");
        }

        return World.CalcPlanetPosition(mars, jdtt);
    }
}
