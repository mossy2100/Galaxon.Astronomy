using System.Diagnostics;

namespace Galaxon.Astronomy;

public delegate double ToJulianDay(DateTime dt);

public class SpeedTest
{
    protected static List<double> DateTimeToJulianDayMethod(string methodName,
        ToJulianDay tjd, List<DateTime> dateTimes)
    {
        Stopwatch sw = new();

        sw.Start();
        List<double> results = dateTimes.Select(tjd.Invoke).ToList();
        sw.Stop();

        Console.WriteLine(
            $"Method {methodName} took {sw.Elapsed.TotalMilliseconds} ms to complete the test.");

        return results;
    }

    private static bool ApproxEqual(double d1, double d2)
    {
        return Abs(d1 - d2) < 1e-9;
    }

    /*
    // Test the ToJulianDay() methods.
    public static void DateTimeToJulianDayMethods()
    {
        Random rnd = new();
        GregorianCalendar gc = new();
        DateTime dt;
        List<DateTime> dateTimes = new();

        // Generate a set of random datetimes, one for each year in the range.
        for (int y = 1; y <= 9999; y++)
        {
            int mon = rnd.Next(1, 13);
            int d = rnd.Next(1, gc.GetDaysInMonth(y, mon) + 1);
            int h = rnd.Next(0, 24);
            int min = rnd.Next(0, 60);
            int s = rnd.Next(0, 60);
            int ms = rnd.Next(0, 1000);
            dt = new(y, mon, d, h, min, s, ms, DateTimeKind.Utc);
            dateTimes.Add(dt);
        }

        // Run speed tests.
        List<double> results1 = DateTimeToJulianDayMethod("ToJulianDay1", DateTimeExtensions.ToJulianDay1, dateTimes);
        List<double> results2 = DateTimeToJulianDayMethod("ToJulianDay2", DateTimeExtensions.ToJulianDay2, dateTimes);
        List<double> results3 = DateTimeToJulianDayMethod("ToJulianDay3", DateTimeExtensions.ToJulianDay3, dateTimes);
        List<double> results4 = DateTimeToJulianDayMethod("ToJulianDay4", DateTimeExtensions.ToJulianDay4, dateTimes);

        for (int i = 0; i < dateTimes.Count; i++)
        {
            bool allMatch = ApproxEqual(results1[i], results2[i]) &&
                ApproxEqual(results2[i], results3[i]) &&
                ApproxEqual(results3[i], results4[i]);
            if (!allMatch)
            {
                Console.WriteLine($"Mismatched results for year {dateTimes[i].Year}: {results1[i]}, {results2[i]}, {results3[i]}, {results4[i]}");
            }
        }
    }
    */
}
