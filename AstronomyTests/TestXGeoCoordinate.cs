using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Astronomy.Algorithms;
using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Numerics.Geometry;
using GeoCoordinatePortable;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestCoordinates
{
    private AstroDbContext? _astroDbContext;

    private AstroObjectRepository? _astroObjectRepository;

    private AstroObjectGroupRepository? _astroObjectGroupRepository;

    [TestInitialize]
    public void Init()
    {
        _astroDbContext = new AstroDbContext();
        _astroObjectGroupRepository = new AstroObjectGroupRepository(_astroDbContext);
        _astroObjectRepository =
            new AstroObjectRepository(_astroDbContext, _astroObjectGroupRepository);
    }

    [TestMethod]
    public void TestShortestDistance()
    {
        // Calculate distance in metres.
        AstroObject? earth = _astroObjectRepository?.Load("Earth", "planet");

        if (earth == null)
        {
            Assert.Fail("Earth could not be found in the database.");
            return;
        }

        // Paris.
        double lat1 = Angle.DmsToDeg(48, 50, 11);
        double long1 = Angle.DmsToDeg(-2, -20, -14);
        GeoCoordinate paris = new (lat1, long1);

        // Washington.
        double lat2 = Angle.DmsToDeg(38, 55, 17);
        double long2 = Angle.DmsToDeg(77, 3, 56);
        GeoCoordinate washington = new (lat2, long2);

        // Calculate distance in metres.
        double dist = DistanceService.ShortestDistanceBetween(paris, washington, earth);

        // Assert.
        // Check it's correct within 5 metres (in the book he's rounded it
        // off to the nearest 10 metres).
        Assert.AreEqual(dist, 6181.63, 0.005);
    }
}
