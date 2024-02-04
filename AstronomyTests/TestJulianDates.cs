using Galaxon.Astronomy.Algorithms.Utilities;
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
        Assert.AreEqual(JulianDateUtility.DateOnly_to_JulianDate(date), 1721425.5);

        // Test current date.
        date = new DateOnly(2022, 6, 8);
        Assert.AreEqual(JulianDateUtility.DateOnly_to_JulianDate(date), 2459738.5);

        // Test middle of range.
        date = new DateOnly(5000, 7, 2);
        Assert.AreEqual(JulianDateUtility.DateOnly_to_JulianDate(date), 3547454.5);

        // Test end of range.
        date = new DateOnly(9999, 12, 31);
        Assert.AreEqual(JulianDateUtility.DateOnly_to_JulianDate(date), 5373483.5);
    }

    [TestMethod]
    public void TestDateOnlyFromJulianDay()
    {
        DateOnly date1, date2;

        // Test start of range.
        date1 = new DateOnly(1, 1, 1);
        date2 = JulianDateUtility.JulianDate_to_DateOnly(1721425.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test current date.
        date1 = new DateOnly(2022, 6, 8);
        date2 = JulianDateUtility.JulianDate_to_DateOnly(2459738.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test middle of range.
        date1 = new DateOnly(5000, 7, 2);
        date2 = JulianDateUtility.JulianDate_to_DateOnly(3547454.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());

        // Test end of range.
        date1 = new DateOnly(9999, 12, 31);
        date2 = JulianDateUtility.JulianDate_to_DateOnly(5373483.5);
        Assert.AreEqual(date1.GetTicks(), date2.GetTicks());
    }

    // TODO Include a reference to the source of the test data.
    [TestMethod]
    public void TestDateTimeToJulianDay()
    {
        DateTime dt;

        // Test start of range.
        dt = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 1721425.5);

        // Test current date.
        dt = new DateTime(2022, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2459737.5);

        // Test middle of range.
        dt = new DateTime(5000, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 3547454.5);

        // Test end of range.
        dt = new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 5373483.5);

        // Test values from Meeus p62.
        dt = new DateTime(2000, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2451545.0);
        dt = new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2451179.5);
        dt = new DateTime(1987, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2446822.5);
        dt = new DateTime(1987, 6, 19, 12, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2446966.0);
        dt = new DateTime(1988, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2447187.5);
        dt = new DateTime(1988, 6, 19, 12, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2447332.0);
        dt = new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2415020.5);
        dt = new DateTime(1600, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2305447.5);
        dt = new DateTime(1600, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc);
        Assert.AreEqual(JulianDateUtility.DateTime_to_JulianDate(dt), 2305812.5);
    }

    // TODO Include a reference to the source of the test data.
    [TestMethod]
    public void TestDateTimeFromJulianDay()
    {
        DateTime dt1, dt2;

        dt1 = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dt2 = JulianDateUtility.JulianDate_to_DateTime(1721425.5);
        Assert.AreEqual(dt1.Ticks, dt2.Ticks);

        dt1 = new DateTime(2022, 6, 7, 0, 0, 0, 0, DateTimeKind.Utc);
        dt2 = JulianDateUtility.JulianDate_to_DateTime(2459737.5);
        Assert.AreEqual(dt1.Ticks, dt2.Ticks);

        dt1 = new DateTime(5000, 7, 2, 0, 0, 0, 0, DateTimeKind.Utc);
        dt2 = JulianDateUtility.JulianDate_to_DateTime(3547454.5);
        Assert.AreEqual(dt1.Ticks, dt2.Ticks);

        dt1 = new DateTime(9999, 12, 31, 0, 0, 0, 0, DateTimeKind.Utc);
        dt2 = JulianDateUtility.JulianDate_to_DateTime(5373483.5);
        Assert.AreEqual(dt1.Ticks, dt2.Ticks);
    }
}
