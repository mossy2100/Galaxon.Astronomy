using System.Data;
using System.Globalization;
using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Numbers;
using Galaxon.Core.Time;
using Galaxon.Numerics.Algebra;

namespace Galaxon.Astronomy.Algorithms;

public class TimeScaleService
{
    /// <summary>
    /// Number of seconds difference between TAI and TT.
    /// TT = TAI + 32.184
    /// </summary>
    public const double TT_MINUS_TAI = 32.184;

    /// <summary>
    /// The start point of the J2000 epoch as a Julian Date (TT).
    /// The number of ephemeris days difference between the start of the Julian epoch and
    /// the start of the J2000 epoch.
    /// </summary>
    public const double J2000_JULIAN_DATE = 2451545.0;

    /// <summary>
    /// The start point of the J2000 epoch in UTC.
    /// This is equal to the Julian Date 2451545.0 TT, i.e. noon on 2000-01-01
    /// in Terrestrial Time.
    /// <see href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#Julian_years_and_J2000"/>
    /// </summary>
    /// <returns>A DateTime object representing the start point of the J2000 epoch in UTC.</returns>
    public static DateTime J2000_UTC { get; } =
        new (2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);

    /// <summary>
    /// Converts a Gregorian DateTime into a single value representing the year with a fractional
    /// part indicating position in the year.
    /// </summary>
    /// <param name="dt">The datetime.</param>
    /// <returns>The year as a double.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the year, month, or day
    /// is invalid.</exception>
    public static double CalcDecimalYear(DateTime dt)
    {
        // Calculate fraction of year.
        GregorianCalendar gc = new ();
        double fracYear = (dt.DayOfYear - 1.0) / gc.GetDaysInYear(dt.Year);

        // Calculate result.
        return dt.Year + fracYear + dt.TimeOfDay.TotalDays;
    }

    /// <summary>
    /// Calculate ∆T in seconds for a given year, month, or date using NASA's
    /// equations.
    /// If only the year is provided, ∆T at the start of the year is
    /// calculated.
    /// If the year and month are provided but no day, then ∆T for the start of
    /// the month is calculated.
    /// If the year, month, and day are provided, then ∆T for the start of
    /// that date is calculated.
    /// ∆T is the difference between Terrestrial Time (TT; formerly known as
    /// Terrestrial Dynamical Time, TDT, or Dynamical Time, TD) and Universal
    /// Time (UT1; also commonly referred to simply as UT).
    /// Thus: ∆T = TT - UT1
    /// Equations in this method were copied from:
    /// <see href="https://eclipse.gsfc.nasa.gov/SEcat5/deltatpoly.html"/>
    /// These are intended to be a simpler method for calculating ∆T than using
    /// tables as in Meeus AA2 and other sources.
    /// Estimates of ∆T are assumed to be reasonably accurate in the range
    /// 1620..2100, but since ∆T varies unpredictably, uncertainty in ∆T
    /// increases outside of this range.
    /// <see href="https://eclipse.gsfc.nasa.gov/SEcat5/uncertainty.html"/>
    /// <see href="https://maia.usno.navy.mil/products/deltaT"/>
    /// <see href="https://asa.hmnao.com/SecK/DeltaT.html"/>
    /// <see href="https://www.hermetic.ch/cal_stud/meeus1.htm"/>
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month. Defaults to 1.</param>
    /// <param name="day">The day of the month. Defaults to 1.</param>
    /// <returns>The calculated value for ∆T.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the year, month, or day
    /// is invalid.</exception>
    public static double CalcDeltaT(int year, int month = 1, int day = 1)
    {
        double u, deltaT;

        // Get the year as a double.
        // This will throw an exception if the year, month, or day is invalid.
        double y = CalcDecimalYear(new DateTime(year, month, day));

        // Calculate deltaT.
        switch (year)
        {
            case < -500:
                u = (y - 1820) / 100;
                deltaT = -20 + 32 * u * u;
                break;

            case -500:
                deltaT = 17203.7;
                break;

            case > -500 and <= 500:
                deltaT = Polynomials.EvaluatePolynomial([
                    10583.6,
                    -1014.41,
                    33.78311,
                    5.952053,
                    -0.1798452,
                    0.022174192,
                    0.0090316521
                ], y / 100);
                break;

            case > 500 and <= 1600:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    1574.2,
                    -556.01,
                    71.23472,
                    0.319781,
                    -0.8503463,
                    -0.005050998,
                    0.0083572073
                }, (y - 1000) / 100);
                break;

            case > 1600 and <= 1700:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    120,
                    -0.9808,
                    -0.01532,
                    1.0 / 7129
                }, y - 1600);
                break;

            case > 1700 and <= 1800:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    8.83,
                    0.1603,
                    -0.0059285,
                    0.00013336,
                    -1.0 / 1174000
                }, y - 1700);
                break;

            case > 1800 and <= 1860:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    13.72,
                    -0.332447,
                    0.0068612,
                    0.0041116,
                    -0.00037436,
                    0.0000121272,
                    -0.0000001699,
                    0.000000000875
                }, y - 1800);
                break;

            case > 1860 and <= 1900:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    7.62,
                    0.5737,
                    -0.251754,
                    0.01680668,
                    -0.0004473624,
                    1.0 / 233174
                }, y - 1860);
                break;

            case > 1900 and <= 1920:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    -2.79,
                    1.494119,
                    -0.0598939,
                    0.0061966,
                    -0.000197
                }, y - 1900);
                break;

            case > 1920 and <= 1941:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    21.20,
                    0.84493,
                    -0.0761,
                    0.0020936
                }, y - 1920);
                break;

            case > 1941 and <= 1961:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    29.07,
                    0.407,
                    -1.0 / 233,
                    1.0 / 2547
                }, y - 1950);
                break;

            case > 1961 and <= 1986:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    45.45,
                    1.067,
                    -1.0 / 260,
                    -1.0 / 718
                }, y - 1975);
                break;

            case > 1986 and <= 2005:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    63.86,
                    0.3345,
                    -0.060374,
                    0.0017275,
                    0.000651814,
                    0.00002373599
                }, y - 2000);
                break;

            case > 2005 and <= 2050:
                deltaT = Polynomials.EvaluatePolynomial(new[]
                {
                    62.92,
                    0.32217,
                    0.005589
                }, y - 2000);
                break;

            case > 2050 and <= 2150:
                u = (y - 1820) / 100;
                deltaT = -20 + 32 * u * u - 0.5628 * (2150 - y);
                break;

            case > 2150:
                u = (y - 1820) / 100;
                deltaT = -20 + 32 * u * u;
                break;
        }

        // Apply the lunar ephemeris correction for years outside the range
        // 1955..2005.
        if (year is < 1955 or > 2005)
        {
            double t = y - 1955;
            deltaT -= 0.000012932 * t * t;
        }

        return deltaT;
    }

    /// <summary>
    /// Calculate ∆T for a given year, month, or date using the method from
    /// Astronomical Algorithms 2nd ed. (AA2) by Jean Meeus, pp77-80.
    /// This pretty closely tracks the NASA values for the range given in the
    /// table (1620-1998) but diverges significantly before and after that.
    /// I implemented this algorithm to compare the two, but I expect the NASA
    /// version is superior.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month. Defaults to 1.</param>
    /// <param name="day">The day of the month. Defaults to 1.</param>
    /// <returns>The calculated value for ∆T.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the year, month, or day is invalid.</exception>
    /// <exception cref="DataException">If a ∆T entry expected to be found in
    /// the database table could not be found.</exception>
    public static double CalcDeltaTMeeus(int year, int month = 1, int day = 1)
    {
        // Get the year as a double.
        double y = TimeScaleService.CalcDecimalYear(new DateTime(year, month, day));

        using AstroDbContext db = new ();

        // Calculate deltaT.
        double deltaT;
        double t = (y - 2000) / 100.0;

        switch (year)
        {
            case < 948:
                deltaT = 2177 + 497 * t + 44.1 * t * t;
                break;

            case >= 948 and < 1620:
            case >= 2000:
                deltaT = 102 + 102 * t + 25.3 * t * t;
                if (y is >= 2000 and <= 2100)
                {
                    deltaT += 0.37 * (y - 2100);
                }
                break;

            case >= 1620 and < 2000:
                // Check if the value is an even integer.
                var y1 = (int)(Floor(y / 2) * 2);
                if (y.FuzzyEquals(y1))
                {
                    // Get this entry from the database.
                    DeltaTRecord? deltaTEntry =
                        db.DeltaTRecords.FirstOrDefault(entry => entry.Year == y1);
                    if (deltaTEntry == null)
                    {
                        throw new DataNotFoundException(
                            $"Could not find delta-T entry for year {y1} in the database.");
                    }
                    deltaT = deltaTEntry.DeltaT;
                }
                else
                {
                    // Interpolate.
                    // Get the deltaT values for the even-numbered years below and above y.
                    double deltaT1 = CalcDeltaTMeeus(y1);
                    double deltaT2 = CalcDeltaTMeeus(y1 + 2);
                    deltaT = (y - y1) / 2 * (deltaT2 - deltaT1) + deltaT1;
                }
                break;
        }

        return deltaT;
    }

    /// <summary>
    /// Calculate the value for ∆T in seconds at a given point in time.
    /// Defaults to current DateTime.
    /// ∆T = TT - UT1
    /// </summary>
    /// <param name="dt">A point in time. Defaults to current DateTime.</param>
    /// <returns></returns>
    public static double CalcDeltaT(DateTime dt = new ())
    {
        return CalcDeltaT(dt.Year, dt.Month, dt.Day);
    }

    public static double CalcDeltaT(double jd)
    {
        return CalcDeltaT(XDateTime.FromJulianDate(jd));
    }

    /// <summary>
    /// Given a Julian Date in Universal Time (JD), find the equivalent in
    /// Terrestrial Time (also known as the Julian Ephemeris Day, or JDE).
    /// ∆T = TT - UT  =>  TT = UT + ∆T
    /// </summary>
    /// <param name="jdut">Julian Date in Universal Time</param>
    /// <returns>Julian Date in Terrestrial Time</returns>
    public static double JulianDateUniversalToTerrestrial(double jdut)
    {
        return jdut + TimeSpan.FromSeconds(CalcDeltaT(jdut)).TotalDays;
    }

    /// <summary>
    /// Convert a Julian Date in Terrestrial Time (TT) (also known as the
    /// Julian Ephemeris Day or JDE) to a Julian Date in Universal Time (JD).
    /// ∆T = TT - UT  =>  UT = TT - ∆T
    /// </summary>
    /// <param name="jdtt">Julian Date in Terrestrial Time</param>
    /// <returns>Julian Date in Universal Time</returns>
    public static double JulianDateTerrestrialToUniversal(double jdtt)
    {
        // Calculate deltaT using TT, it should be virtually identical to UT.
        double deltaT = CalcDeltaT(jdtt);
        return jdtt - TimeSpan.FromSeconds(deltaT).TotalDays;
    }

    public static double DateTimeToJulianDateTerrestrial(DateTime dtut)
    {
        return JulianDateUniversalToTerrestrial(dtut.ToJulianDate());
    }

    public static DateTime JulianDateTerrestrialToDateTime(double jdtt)
    {
        return XDateTime.FromJulianDate(JulianDateTerrestrialToUniversal(jdtt));
    }

    /// <summary>
    /// Find out how many leap seconds there were or have been prior to the
    /// given instant.
    /// Get the latest info about leap seconds from IERS Bulletin A:
    /// <see href="https://www.iers.org/IERS/EN/Publications/Bulletins/bulletins.html"/>
    /// </summary>
    /// <param name="dt">A point in time. Defaults to current DateTime.</param>
    /// <returns>The number of leap seconds until then.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If year less than
    /// 1972.</exception>
    public static byte LeapSecondCount(DateTime dt = new ())
    {
        // Check for valid year.
        if (dt.Year < 1972)
        {
            throw new ArgumentOutOfRangeException(nameof(dt),
                "The leap second count is only relevant from 1972, when they were introduced.");
        }

        // Count the leap seconds preceding the current date.
        return (byte)LeapSecond.List.Count(ls => ls.Date.ToDateTime() < dt);
    }

    /// <summary>
    /// Find the difference between TAI and UTC at a given point in time.
    /// </summary>
    /// <param name="dt">A point in time. Defaults to current
    /// DateTime.</param>
    /// <returns>The integer number of seconds difference.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If year less than 1972.</exception>
    public static byte CalcTAIMinusUTC(DateTime dt = new ())
    {
        return (byte)(10 + LeapSecondCount(dt));
    }

    /// <summary>
    /// Calculate the difference between UT1 and UTC in seconds.
    /// DUT1 = UT1 - UTC
    /// In theory, this should always be between -0.9 and 0.9. However, because
    /// the CalcDeltaT() algorithm is only approximate, it isn't.
    /// DUT1 is normally measured in retrospect, not calculated. This
    /// calculation has been included to check the accuracy of the CalcDeltaT()
    /// method.
    /// This calculation of DUT1 is only valid within the nominal range from
    /// 1972..2010. Yet the *actual* DUT1 is within range up until 2022 (the
    /// time of writing) because leap seconds have been added to produce exactly
    /// this effect. The error must be in CalcDeltaT(), which leads me to
    /// believe the NASA formulae used in this library either aren't super
    /// accurate or (more likely) they were developed before 2010.
    /// <see href="https://en.wikipedia.org/wiki/DUT1"/>
    /// </summary>
    /// <param name="dt">A point in time. Defaults to current
    /// DateTime.</param>
    /// <returns>The difference between UT1 and UTC.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If year less than 1972.</exception>
    public static double CalcDUT1(DateTime dt = new ())
    {
        return TT_MINUS_TAI - CalcDeltaT(dt) + CalcTAIMinusUTC(dt);
    }

    /// <summary>
    /// Number of days since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianDaysSinceJ2000(double jdtt)
    {
        return jdtt - J2000_JULIAN_DATE;
    }

    /// <summary>
    /// Number of Julian years since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianYearsSinceJ2000(double jdtt)
    {
        return JulianDaysSinceJ2000(jdtt) / XTimeSpan.DAYS_PER_JULIAN_YEAR;
    }

    /// <summary>
    /// Number of Julian centuries since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianCenturiesSinceJ2000(double jdtt)
    {
        return JulianDaysSinceJ2000(jdtt) / XTimeSpan.DAYS_PER_JULIAN_CENTURY;
    }

    /// <summary>
    /// Number of Julian millennia since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianMillenniaSinceJ2000(double jdtt)
    {
        return JulianDaysSinceJ2000(jdtt) / XTimeSpan.DAYS_PER_JULIAN_MILLENNIUM;
    }

    // The goal here is to generate a chart to show the difference between the 2 methods.
    // Chart 1 would show the full range -1999..3000.
    // Chart 2 would show the narrow (more accurate) range 1600..2100
    // I will use a Syncfusion component for the chart, and add an ASP.NET Core project to display it.
    public static void CompareDeltaTCalcs()
    {
        for (int y = -1999; y <= 3000; y++)
        {
            double deltaTNasa = CalcDeltaT(y);
            double deltaTMeeus = TimeScaleService.CalcDeltaTMeeus(y);
            double diff = Abs(deltaTMeeus - deltaTNasa);
        }
    }

    public static void TestCalcDUT1()
    {
        for (var y = 1972; y <= 2022; y++)
        {
            var dt = new DateTime(y, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            byte LSC = LeapSecondCount(dt);
            double deltaT = CalcDeltaT(dt);
            double DUT1 = CalcDUT1(dt);
            Console.WriteLine($"Year={y}, LSC={LSC}, ∆T={deltaT}, DUT1={DUT1}");
            if (Abs(DUT1) > 0.9)
            {
                Console.WriteLine("Wrong.");
            }
        }
    }
}
