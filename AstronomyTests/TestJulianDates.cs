using Galaxon.Astronomy.Algorithms;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestJulianDates
{
    [TestMethod]
    public void TestDateOnlyToJulianDay()
    {
        DateOnly date;

        // Test start of range.
        date = new DateOnly(1, 1, 1);
        Assert.AreEqual(JulianDateService.DateOnly_to_JulianDate(date), 1721425.5);

        // Test current date.
        date = new DateOnly(2022, 6, 8);
        Assert.AreEqual(JulianDateService.DateOnly_to_JulianDate(date), 2459738.5);

        // Test middle of range.
        date = new DateOnly(5000, 7, 2);
        Assert.AreEqual(JulianDateService.DateOnly_to_JulianDate(date), 3547454.5);

        // Test end of range.
        date = new DateOnly(9999, 12, 31);
        Assert.AreEqual(JulianDateService.DateOnly_to_JulianDate(date), 5373483.5);
    }

    [TestMethod]
    public void TestDateOnlyFromJulianDay()
    {
        DateOnly date1, date2;

        // Test start of range.
        date1 = new DateOnly(1, 1, 1);
        date2 = JulianDateService.JulianDate_to_DateOnly(1721425.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test current date.
        date1 = new DateOnly(2022, 6, 8);
        date2 = JulianDateService.JulianDate_to_DateOnly(2459738.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test middle of range.
        date1 = new DateOnly(5000, 7, 2);
        date2 = JulianDateService.JulianDate_to_DateOnly(3547454.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test end of range.
        date1 = new DateOnly(9999, 12, 31);
        date2 = JulianDateService.JulianDate_to_DateOnly(5373483.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());
    }
}
