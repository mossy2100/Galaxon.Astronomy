using Galaxon.Astronomy.Repository.Enums;
using Galaxon.Core.Testing;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestSeasonalMarker
{
    [TestMethod]
    public void TestCalcApproxSeasonalMarker()
    {
        // Test Example 27.a from AA2 p180.
        DateTime dt = Terran.CalcSeasonalMarkerApprox(1962, ESeasonalMarker.JuneSolstice);
        var dt2 = new DateTime(1962, 6, 21, 21, 25, 8);
        // Check they match within 1 second.
        var delta = new TimeSpan(TimeSpan.TicksPerSecond);
        XAssert.AreEqual(dt, dt2, delta);
    }
}
