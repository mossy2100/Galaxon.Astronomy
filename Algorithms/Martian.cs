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
    public const double DaysPerSol = 1.02749;

    public static double CalcMarsSolDate(double jd) => (jd - 2405522.0) / DaysPerSol;

    public static Planet? GetPlanet()
    {
        using AstroDbContext db = new();
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
