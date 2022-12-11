using AstroMultimedia.Core.Testing;
using AstroMultimedia.Quantities;

namespace AstroMultimedia.Astronomy.Tests;

[TestClass]
public class TestSeasonalMarker
{
    [TestMethod]
    public void TestCalcApproxSeasonalMarker()
    {
        // Test Example 27.a from AA2 p180.
        DateTime dt = Terran.CalcSeasonalMarkerApprox(1962, ESeasonalMarker.JuneSolstice);
        DateTime dt2 = new DateTime(1962, 6, 21, 21, 25, 8);
        // Check they match within 1 second.
        TimeSpan delta = new TimeSpan((long)TimeSpan.TicksPerSecond);
        XAssert.AreEqual(dt, dt2, delta);
    }
}
