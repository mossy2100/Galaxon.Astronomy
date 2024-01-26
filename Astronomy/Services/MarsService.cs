using Galaxon.Astronomy.Database;
using Galaxon.Astronomy.Models;

namespace Galaxon.Astronomy.Services;

/// <summary>
/// A container for constants and static methods related to Mars.
/// </summary>
public class MarsService
{
    private AstroObjectRepository _repo;

    private AstroObject? _mars;

    /// <summary>
    /// Number of days (Earth solar days) per sol (Mars solar day).
    /// </summary>
    public const double DAYS_PER_SOL = 1.02749;

    public MarsService(AstroObjectRepository repo)
    {
        _repo = repo;
    }

    public AstroObject GetPlanet()
    {
        if (_mars == null)
        {
            // TODO make this call async
            _mars = _repo.Load("Mars");
        }
        return _mars;
    }

    public static double CalcMarsSolDate(double jd)
    {
        return (jd - 2405522.0) / DAYS_PER_SOL;
    }

    public (double L, double B, double R) CalcPosition(double jdtt)
    {
        AstroObject mars = GetPlanet();
        return PlanetService.CalcPlanetPosition(mars, jdtt);
    }
}
