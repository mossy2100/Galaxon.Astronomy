using AstroMultimedia.Core.Time;
using AstroMultimedia.Numerics.Geometry;
using AstroMultimedia.Quantities;

namespace AstroMultimedia.Astronomy.Tests;

[TestClass]
public class TestPlanets
{
    /// <summary>
    /// Test the example given in Wikipedia.
    /// <see href="https://en.wikipedia.org/wiki/Sidereal_time#ERA"/>
    /// </summary>
    [TestMethod]
    public void TestERA()
    {
        DateTime dt = new(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double expected = Angle.DmsToRad(100, 37, 12.4365);
        double actual = Terran.CalcERA(dt);
        double delta = Angle.DmsToRad(0, 0, 1e-3);
        Assert.AreEqual(expected, actual, delta);
    }

    /// <summary>
    /// Test Example 32.a from AA2 p219.
    /// </summary>
    [TestMethod]
    public void TestCalcPositionVenus()
    {
        // Arrange.
        using AstroDbContext db = new();
        Planet? venus = Planet.Load(db, "Venus");
        if (venus == null)
        {
            Assert.Fail("Could not find Venus in the database.");
            return;
        }

        double expectedL = Angle.NormalizeRadians(-68.659_258_2);
        double expectedB = Angle.NormalizeRadians(-0.045_739_9);
        double expectedR = 0.724_603;

        // Act.
        (double actualL, double actualB, double actualR) = World.CalcPlanetPosition(venus, 2_448_976.5);

        // Assert.
        // I assume larger delta values are needed here because Meeus uses a
        // subset of terms from VSOP87 whereas this library uses all of them,
        // thereby producing a more accurate result.
        Assert.AreEqual(expectedL, actualL, 1e-5);
        Assert.AreEqual(expectedB, actualB, 1e-5);
        Assert.AreEqual(expectedR, actualR / Length.MetresPerAu, 1e-5);
    }

    /// <summary>
    /// Test Example 32.b from AA2 p219.
    /// </summary>
    [TestMethod]
    public void TestCalcPositionSaturn()
    {
        // Arrange.
        using AstroDbContext db = new();
        Planet? saturn = Planet.Load(db, "Saturn");
        if (saturn == null)
        {
            Assert.Fail("Could not find Saturn in the database.");
            return;
        }
        DateTime dttt = new DateTime(1999, 7, 26, 0, 0, 0, DateTimeKind.Utc);
        double jdtt = dttt.ToJulianDay();

        // Act.
        (double actualL, double B, double R) = World.CalcPlanetPosition(saturn, jdtt);

        // Assert.
        double expectedL = Angle.DegToRad(39.972_3901);
        Assert.AreEqual(expectedL, actualL, 1e-6);
    }
}
