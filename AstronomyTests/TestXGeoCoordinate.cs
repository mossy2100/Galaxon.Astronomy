using AstroMultimedia.Numerics.Geometry;
using GeoCoordinatePortable;

namespace AstroMultimedia.Astronomy.Tests;

[TestClass]
public class TestCoordinates
{
    [TestMethod]
    public void TestShortestDistance()
    {
        // Calculate distance in metres.
        using AstroDbContext db = new();
        Planet? earth = Planet.Load(db, "Earth");

        if (earth == null)
        {
            Assert.Fail("Earth could not be found in the database.");
            return;
        }

        // Paris.
        double lat1 = Angle.DmsToDeg(48, 50, 11);
        double long1 = Angle.DmsToDeg(-2, -20, -14);
        GeoCoordinate paris = new(lat1, long1);

        // Washington.
        double lat2 = Angle.DmsToDeg(38, 55, 17);
        double long2 = Angle.DmsToDeg(77, 3, 56);
        GeoCoordinate washington = new(lat2, long2);

        // Calculate distance in metres.
        double dist = World.ShortestDistanceBetween(paris, washington, earth);

        // Assert.
        // Check it's correct within 5 metres (in the book he's rounded it
        // off to the nearest 10 metres).
        Assert.AreEqual(dist, 6181.63, 0.005);
    }
}
