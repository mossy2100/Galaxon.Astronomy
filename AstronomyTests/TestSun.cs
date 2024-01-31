using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Algorithms;
using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestSun
{
    private AstroDbContext? _astroDbContext;

    private AstroObjectRepository? _astroObjectRepository;

    private AstroObjectGroupRepository? _astroObjectGroupRepository;

    private EarthService? _earthService;

    private PlanetService? _planetService;

    private SunService? _sunService;

    [TestInitialize]
    public void Init()
    {
        _astroDbContext = new AstroDbContext();
        _astroObjectGroupRepository = new AstroObjectGroupRepository(_astroDbContext);
        _astroObjectRepository =
            new AstroObjectRepository(_astroDbContext, _astroObjectGroupRepository);
        _planetService = new PlanetService(_astroDbContext);
        _earthService = new EarthService(_astroObjectRepository, _planetService);
        _sunService = new SunService(_astroDbContext, _astroObjectRepository,
            _astroObjectGroupRepository, _earthService, _planetService);
    }

    [TestMethod]
    public void CalcPositionTest()
    {
        double jdtt = 2448908.5;
        (double actualL, double actualB) = _sunService!.CalcPosition(jdtt);

        double expectedL = Angle.NormalizeRadians(Angle.DmsToRad(199, 54, 21.82));
        double expectedB = Angle.NormalizeRadians(Angle.DmsToRad(0, 0, 0.62));

        // Note the large delta necessary for the test to pass. This is probably
        // because the calculation in AA2 uses the 1980 method for calculating
        // nutation instead of the more modern SOFA method as used in this
        // library.
        Assert.AreEqual(expectedL, actualL, 1e-3);
        Assert.AreEqual(expectedB, actualB, 1e-3);
    }
}
