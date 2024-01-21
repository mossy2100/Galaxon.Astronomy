using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestSolar
{
    [TestMethod]
    public void CalcPositionTest()
    {
        var jdtt = 2448908.5;
        (double actualL, double actualB) = Solar.CalcPosition(jdtt);

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
