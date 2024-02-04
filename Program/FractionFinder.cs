using Galaxon.Core.Time;

namespace Galaxon.Astronomy;

public class FractionFinder
{
    public static void FindLongMonthFraction()
    {
        double frac = XTimeSpan.DAYS_PER_LUNATION - (int)XTimeSpan.DAYS_PER_LUNATION;
        var epsilon = 0.001;
        double? bestDiff = null;
        for (var d = 1; d < 12369; d++)
        {
            //Console.WriteLine($"Testing d = {d}...");
            var n = (int)Round(frac * d);
            if (n == 0)
            {
                continue;
            }

            double frac2 = n / (double)d;
            double diff = Abs(frac - frac2);
            if (!(diff <= epsilon) || (bestDiff != null && !(diff < bestDiff)))
            {
                continue;
            }

            bestDiff = diff;
            var nSeconds = (int)Round(diff * XTimeSpan.SECONDS_PER_DAY);
            Console.WriteLine(
                $"Better match: {n} / {d} = {frac2} (diff is about {nSeconds} seconds in {d} months)");
            var m = (int)(1 / frac2);
            Console.WriteLine($"Rule: year % {d} % {m} == 0");
        }
    }

    public static bool IsLongMonth(int month)
    {
        var oddMonthsLong = 0;
        var monthsPerCycle = 850;
        var gapLengthMonths = 32;
        var offset = 25;
        return month % 2 == oddMonthsLong || month % monthsPerCycle % gapLengthMonths == offset;
    }

    // @todo Ensure the rule works with negative values for LN.
    public static void TestLongMonthRule()
    {
        var oddMonthsLong = 0;
        var monthsPerCycle = 850;
        var gapLengthMonths = 32;
        var offset = 25;
        Console.WriteLine(
            $"The rule: isLongMonth = (m % 2 == {oddMonthsLong}) || (m % {monthsPerCycle} % {gapLengthMonths} == {offset})");

        var longMonthCount = 0;
        List<int> monthNumbers = [];
        // LN = Lunation Number. LN = 0 started with the first New Moon of 2000, approximately 2000-01-06T18:14+0000.
        for (var month = 0; month < monthsPerCycle; month++)
        {
            if (!IsLongMonth(month))
            {
                continue;
            }

            if (month % 2 != oddMonthsLong)
            {
                monthNumbers.Add(month);
            }

            longMonthCount++;
        }

        Console.WriteLine(
            $"All even months are long, plus these months within the {monthsPerCycle} month cycle: {string.Join(", ", monthNumbers)}");
        double calMonthLengthDays = 29 + longMonthCount / (double)monthsPerCycle;
        Console.WriteLine(
            $"This gives {longMonthCount} long months per {monthsPerCycle} months, which is an average of {calMonthLengthDays} days per month.");
        double driftDaysPerMonth = Abs(calMonthLengthDays - XTimeSpan.DAYS_PER_LUNATION);
        double driftSecondsPerMonth = driftDaysPerMonth * XTimeSpan.SECONDS_PER_DAY;
        Console.WriteLine(
            $"The drift is about {driftDaysPerMonth:N9} days or {driftSecondsPerMonth:N3} seconds per month.");
        double driftSecondsPerDay = driftSecondsPerMonth / calMonthLengthDays;
        double driftSecondsPerYear = driftSecondsPerDay * XTimeSpan.DAYS_PER_TROPICAL_YEAR;
        double cycleLengthDays = monthsPerCycle * calMonthLengthDays;
        double cycleLengthYears = cycleLengthDays / XTimeSpan.DAYS_PER_TROPICAL_YEAR;
        Console.WriteLine($"A cycle length is about {cycleLengthYears:N2} years.");
        double driftSecondsPerCycle = driftSecondsPerMonth * monthsPerCycle;
        Console.WriteLine(
            $"Thus, the drift is about {driftSecondsPerYear:N3} seconds per year, or {driftSecondsPerCycle:N3} seconds per cycle (not counting the changing length of the synodic month, which is increasing by about 0.022 seconds per century.");
    }

    public static void FindLeapYearFraction()
    {
        double frac = XTimeSpan.DAYS_PER_TROPICAL_YEAR - (int)XTimeSpan.DAYS_PER_TROPICAL_YEAR;
        List<double> fractions = [];
        for (var d = 1; d <= 10000; d++)
        {
            // Console.WriteLine($"Testing d = {d}...");
            var n = (int)Round(frac * d);
            if (n == 0)
            {
                continue;
            }

            double frac2 = n / (double)d;

            // Eliminate duplicates.
            if (fractions.Contains(frac2))
            {
                continue;
            }
            fractions.Add(frac2);

            double diffDays = Abs(frac - frac2);
            double diffSeconds = diffDays * XTimeSpan.SECONDS_PER_DAY;
            if (diffSeconds <= 1)
            {
                Console.WriteLine(
                    $"Match: {n} / {d} = {frac2} (drift is about {diffSeconds:N3} seconds per year)");
            }
        }

        Console.WriteLine("Done.");
    }

    public static bool IsLeapYear(int year)
    {
        var yearsPerCycle = 128;
        var gapLengthYears = 4;
        return year % gapLengthYears == 0 && year % yearsPerCycle != 0;
    }

    public static void TestLeapYearRule()
    {
        var leapYearCount = 0;
        var yearsPerCycle = 128;
        var gapLengthYears = 4;
        Console.WriteLine(
            $"The rule: isLeapYear = (m % {gapLengthYears} == 0) && (m % {yearsPerCycle} != 0)");
        List<int> yearNumbers = [];
        for (var y = 0; y < yearsPerCycle; y++)
        {
            if (IsLeapYear(y))
            {
                yearNumbers.Add(y);
                leapYearCount++;
            }
        }

        Console.WriteLine(
            $"The following years within the {yearsPerCycle} year cycle are leap years: {string.Join(", ", yearNumbers)}");
        double calYearLengthDays = 365 + leapYearCount / (double)yearsPerCycle;
        Console.WriteLine(
            $"This gives {leapYearCount} leap years per {yearsPerCycle} years, which is an average of {calYearLengthDays} days per year.");
        double driftDaysPerYear = Abs(calYearLengthDays - XTimeSpan.DAYS_PER_TROPICAL_YEAR);
        double driftSecondsPerYear = driftDaysPerYear * XTimeSpan.SECONDS_PER_DAY;
        double driftSecondsPerCycle = driftSecondsPerYear * yearsPerCycle;
        Console.WriteLine(
            $"The drift is about {driftSecondsPerYear:N3} seconds per year, or {driftSecondsPerCycle:N3} seconds per cycle (not counting the changing length of the tropical year, which is decreasing by about 0.53 seconds per century).");
    }

    public static void TestLeapYearRule2()
    {
        var leapYearCount = 0;
        var a = 9005;
        var b = 4;
        var c = 128;
        Console.WriteLine($"The rule: isLeapYear = (y % {a} % {b} == 0) && (y % {a} % {c} != 0)");
        List<int> yearNumbers = [];
        for (var y = 0; y < a; y++)
        {
            if (IsLeapYear(y % a))
            {
                yearNumbers.Add(y);
                leapYearCount++;
            }
        }

        Console.WriteLine(
            $"The following years within the {a} year cycle are leap years: {string.Join(", ", yearNumbers)}");
        double calYearLengthDays = 365 + leapYearCount / (double)a;
        Console.WriteLine(
            $"This gives {leapYearCount} leap years per {a} years, which is an average of {calYearLengthDays} days per year.");
        double driftDaysPerYear = Abs(calYearLengthDays - XTimeSpan.DAYS_PER_TROPICAL_YEAR);
        double driftSecondsPerYear = driftDaysPerYear * XTimeSpan.SECONDS_PER_DAY;
        double driftSecondsPerCycle = driftSecondsPerYear * a;
        Console.WriteLine(
            $"The drift is about {driftSecondsPerYear:N3} seconds per year, or {driftSecondsPerCycle:N3} seconds per cycle (not counting the changing length of the tropical year, which is decreasing by about 0.53 seconds per century).");
    }
}
