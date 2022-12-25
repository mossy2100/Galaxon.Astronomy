using System.Data;
using System.Globalization;
using Galaxon.Astronomy.Repository;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Numbers;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;
using Galaxon.Numerics.Maths;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// This is a static class containing useful methods and constants relating to
/// time.
///
/// This includes methods and constants for converting between time scales,
/// including:
/// - Terrestrial Time (TT)
/// - Universal Time (UT1)
/// - Coordinated Universal Time (UTC)
/// - International Atomic Time (TAI)
/// Differences are calculated in SI seconds in all cases.
/// </summary>
public static class Terran
{
    #region Constants

    /// <summary>
    /// Number of seconds difference between TAI and TT.
    /// TT = TAI + 32.184
    /// </summary>
    public const double TT_MINUS_TAI = 32.184;

    /// <summary>
    /// The number of days difference between the start of the Julian epoch and
    /// the start of the J2000 epoch.
    /// </summary>
    public const double DAYS_SINCE_J2000 = 2451545.0;

    #endregion Constants

    /// <summary>
    /// Converts a Gregorian date into a single value: the year with a
    /// fractional part indicating position in the year.
    /// The values must represent a valid date.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day of the month.</param>
    /// <returns>The year as a double.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the year, month, or day
    /// is invalid.</exception>
    public static double CalcDecimalYear(int year, int month, int day)
    {
        // Check year is valid.
        // Meeus doesn't specify a year range for his method, so let's just use
        // the same range for both methods, which comes from the NASA algorithm.
        if (year is < -1999 or > 3000)
        {
            throw new ArgumentOutOfRangeException(nameof(year),
                "Year must be in the range -1999..3000");
        }

        // Check month is valid.
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month),
                "Month must be in the range 1..12");
        }

        // Check day is valid.
        GregorianCalendar gc = new();
        int daysInMonth = gc.GetDaysInMonth(year, month);
        if (day < 1 || day > daysInMonth)
        {
            throw new ArgumentOutOfRangeException(nameof(day),
                $"Day must be in the range 1..{daysInMonth}");
        }

        // Calculate fraction of year.
        DateTime dt = new(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        double frac = (dt.DayOfYear - 1.0) / gc.GetDaysInYear(year);
        return year + frac;
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
        double y = CalcDecimalYear(year, month, day);

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
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    10583.6,
                    -1014.41,
                    33.78311,
                    5.952053,
                    -0.1798452,
                    0.022174192,
                    0.0090316521
                }, y / 100);
                break;

            case > 500 and <= 1600:
                deltaT = Equations.EvaluatePolynomial(new[]
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
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    120,
                    -0.9808,
                    -0.01532,
                    1.0 / 7129
                }, y - 1600);
                break;

            case > 1700 and <= 1800:
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    8.83,
                    0.1603,
                    -0.0059285,
                    0.00013336,
                    -1.0 / 1174000
                }, y - 1700);
                break;

            case > 1800 and <= 1860:
                deltaT = Equations.EvaluatePolynomial(new[]
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
                deltaT = Equations.EvaluatePolynomial(new[]
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
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    -2.79,
                    1.494119,
                    -0.0598939,
                    0.0061966,
                    -0.000197
                }, y - 1900);
                break;

            case > 1920 and <= 1941:
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    21.20,
                    0.84493,
                    -0.0761,
                    0.0020936
                }, y - 1920);
                break;

            case > 1941 and <= 1961:
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    29.07,
                    0.407,
                    -1.0 / 233,
                    1.0 / 2547
                }, y - 1950);
                break;

            case > 1961 and <= 1986:
                deltaT = Equations.EvaluatePolynomial(new[]
                {
                    45.45,
                    1.067,
                    -1.0 / 260,
                    -1.0 / 718
                }, y - 1975);
                break;

            case > 1986 and <= 2005:
                deltaT = Equations.EvaluatePolynomial(new[]
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
                deltaT = Equations.EvaluatePolynomial(new[]
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
    /// Calculate the value for ∆T in seconds at a given point in time.
    /// Defaults to current DateTime.
    /// ∆T = TT - UT1
    /// </summary>
    /// <param name="dt">A point in time. Defaults to current DateTime.</param>
    /// <returns></returns>
    public static double CalcDeltaT(DateTime dt = new DateTime()) =>
        CalcDeltaT(dt.Year, dt.Month, dt.Day);

    public static double CalcDeltaT(double jd) => CalcDeltaT(XDateTime.FromJulianDay(jd));

    /// <summary>
    /// Given a Julian Day in Universal Time (JD), find the equivalent in
    /// Terrestrial Time (also known as the Julian Ephemeris Day, or JDE).
    /// ∆T = TT - UT  =>  TT = UT + ∆T
    /// </summary>
    /// <param name="jdut">Julian Day value in Universal Time</param>
    /// <returns>Julian Day in Terrestrial Time</returns>
    public static double JulianDayUniversalToTerrestrial(double jdut) =>
        jdut + TimeSpan.FromSeconds(CalcDeltaT(jdut)).TotalDays;

    /// <summary>
    /// Convert a Julian Day value in Terrestrial Time (TT) (also known as the
    /// Julian Ephemeris Day or JDE) to a Julian Day in Universal Time (JD).
    /// ∆T = TT - UT  =>  UT = TT - ∆T
    /// </summary>
    /// <param name="jdtt">Julian Day in Terrestrial Time</param>
    /// <returns>Julian Day value in Universal Time</returns>
    public static double JulianDayTerrestrialToUniversal(double jdtt) =>
        jdtt - TimeSpan.FromSeconds(CalcDeltaT(jdtt)).TotalDays;

    public static double DateTimeUniversalToJulianDayTerrestrial(DateTime dtut) =>
        JulianDayUniversalToTerrestrial(dtut.ToJulianDay());

    public static DateTime JulianDayTerrestrialToDateTimeUniversal(double jdtt) =>
        XDateTime.FromJulianDay(JulianDayTerrestrialToUniversal(jdtt));

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
    public static byte LeapSecondCount(DateTime dt = new DateTime())
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
    public static byte CalcTAIMinusUTC(DateTime dt = new ()) =>
        (byte)(10 + LeapSecondCount(dt));

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
    public static double CalcDUT1(DateTime dt = new DateTime()) =>
        TT_MINUS_TAI - CalcDeltaT(dt) + CalcTAIMinusUTC(dt);

    /// <summary>
    /// The start point of the J2000 epoch in UTC.
    /// This is equal to the Julian date 2451545.0 TT, i.e. noon on 2000-01-01
    /// in Terrestrial Time.
    /// <see href="https://en.wikipedia.org/wiki/Epoch_(astronomy)#Julian_years_and_J2000"/>
    /// </summary>
    /// <returns>A DateTime object representing the start point of the J2000
    /// epoch in UTC.</returns>
    public static DateTime J2000StartUTC() =>
        new DateTime(2000, 1, 1, 11, 58, 55, 816, DateTimeKind.Utc);

    /// <summary>
    /// Number of days since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianDaysSinceJ2000(double jdtt) => jdtt - DAYS_SINCE_J2000;

    /// <summary>
    /// Number of Julian years since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianYearsSinceJ2000(double jdtt) =>
        JulianDaysSinceJ2000(jdtt) / XTimeSpan.DaysPerJulianYear;

    /// <summary>
    /// Number of Julian centuries since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianCenturiesSinceJ2000(double jdtt) =>
        JulianDaysSinceJ2000(jdtt) / XTimeSpan.DaysPerJulianCentury;

    /// <summary>
    /// Number of Julian millennia since beginning of the J2000.0 epoch, in TT.
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns></returns>
    public static double JulianMillenniaSinceJ2000(double jdtt) =>
        JulianDaysSinceJ2000(jdtt) / XTimeSpan.DaysPerJulianMillennium;

    /// <summary>
    /// Calculate the Earth Rotation Angle from the Julian Day in UT1.
    /// <see href="https://en.wikipedia.org/wiki/Sidereal_time#ERA"/>
    /// TODO This method probably belongs in a different class.
    /// </summary>
    /// <param name="jdut">The Julian Day in UT1.</param>
    /// <returns>The Earth Rotation Angle.</returns>
    public static double CalcERA(double jdut)
    {
        double t = JulianDaysSinceJ2000(jdut);
        double radians = Math.Tau * (0.779_057_273_264 + 1.002_737_811_911_354_48 * t);
        return Angle.NormalizeRadians(radians);
    }

    /// <summary>
    /// Overload of CalcERA() that accepts a UTC DateTime.
    /// </summary>
    /// <param name="dtut">The instant.</param>
    /// <returns>The ERA at the given instant.</returns>
    public static double CalcERA(DateTime dtut) => CalcERA(dtut.ToJulianDay());

    public static Planet? GetPlanet()
    {
        using AstroDbContext db = new();
        return Planet.Load(db, "Earth");
    }

    public static (double L, double B, double R) CalcPosition(double jdtt)
    {
        Planet? earth = GetPlanet();
        if (earth == null)
        {
            throw new DataNotFoundException("Could not find Earth in the database.");
        }
        return World.CalcPlanetPosition(earth, jdtt);
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
        double y = Terran.CalcDecimalYear(year, month, day);

        using AstroDbContext db = new();

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
                int y1 = (int)(Floor(y / 2) * 2);
                if (y.FuzzyEquals(y1))
                {
                    // Get this entry from the database.
                    DeltaTEntry? deltaTEntry =
                        db.DeltaTEntries.FirstOrDefault(entry => entry.Year == y1);
                    if (deltaTEntry == null)
                    {
                        throw new DataException(
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

    // The goal here is to generate a chart to show the difference between the 2 methods.
    // Chart 1 would show the full range -1999..3000.
    // Chart 2 would show the narrow (more accurate) range 1600..2100
    // I will use a Syncfusion component for the chart, and add an ASP.NET Core project to display it.
    public static void CompareDeltaTCalcs()
    {
        for (int y = -1999; y <= 3000; y++)
        {
            double deltaTNasa = Terran.CalcDeltaT(y);
            double deltaTMeeus = CalcDeltaTMeeus(y);
            double diff = Abs(deltaTMeeus - deltaTNasa);
        }
    }

    public static void TestCalcDUT1()
    {
        for (int y = 1972; y <= 2022; y++)
        {
            DateTime dt = new DateTime(y, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            byte LSC = Terran.LeapSecondCount(dt);
            double deltaT = Terran.CalcDeltaT(dt);
            double DUT1 = Terran.CalcDUT1(dt);
            Console.WriteLine($"Year={y}, LSC={LSC}, ∆T={deltaT}, DUT1={DUT1}");
            if (Abs(DUT1) > 0.9)
            {
                Console.WriteLine("Wrong.");
            }
        }
    }

    /// <summary>
    /// Calculate mean value (as JDE0) for a seasonal marker.
    /// Algorithm from AA2 p178.
    /// </summary>
    /// <param name="year">The year (Gregorian) in the range -1000..3000.</param>
    /// <param name="markerNumber">The marker number (use the enum).</param>
    /// <returns></returns>
    public static double CalcSeasonalMarkerMean(int year, ESeasonalMarker markerNumber)
    {
        if (year is < -1000 or > 3000)
        {
            throw new ArgumentOutOfRangeException(nameof(year),
                "Must be in the range -1000..3000.");
        }

        if (markerNumber is < ESeasonalMarker.MarchEquinox or > ESeasonalMarker.DecemberSolstice)
        {
            throw new ArgumentOutOfRangeException(nameof(markerNumber), "Invalid value.");
        }

        double Y;

        if (year <= 1000)
        {
            Y = year / 1000.0;
            return markerNumber switch
            {
                ESeasonalMarker.MarchEquinox =>
                    Equations.EvaluatePolynomial(new[]
                    {
                        1721139.29189,
                        365242.13740,
                        0.06134,
                        0.00111,
                        -0.00071
                    }, Y),
                ESeasonalMarker.JuneSolstice =>
                    Equations.EvaluatePolynomial(new[]
                    {
                        1721233.25401,
                        365241.72562,
                        -0.05323,
                        0.00907,
                        0.00025
                    }, Y),
                ESeasonalMarker.SeptemberEquinox =>
                    Equations.EvaluatePolynomial(new[]
                    {
                        1721325.70455,
                        365242.49558,
                        -0.11677,
                        -0.00297,
                        0.00074
                    }, Y),
                ESeasonalMarker.DecemberSolstice =>
                    Equations.EvaluatePolynomial(new[]
                    {
                        1721414.39987,
                        365242.88257,
                        -0.00769,
                        -0.00933,
                        -0.00006
                    }, Y),
                _ => throw new ArgumentOutOfRangeException(nameof(markerNumber), "Invalid value."),
            };
        }

        Y = (year - 2000) / 1000.0;
        return markerNumber switch
        {
            ESeasonalMarker.MarchEquinox =>
                Equations.EvaluatePolynomial(new[]
                {
                    2451623.80984,
                    365242.37404,
                    0.05169,
                    -0.00411,
                    -0.00057
                }, Y),
            ESeasonalMarker.JuneSolstice =>
                Equations.EvaluatePolynomial(new[]
                {
                    2451716.56767,
                    365241.62603,
                    0.00325,
                    0.00888,
                    -0.0003
                }, Y),
            ESeasonalMarker.SeptemberEquinox =>
                Equations.EvaluatePolynomial(new[]
                {
                    2451810.21715,
                    365242.01767,
                    -0.11575,
                    0.00337,
                    0.00078
                }, Y),
            ESeasonalMarker.DecemberSolstice =>
                Equations.EvaluatePolynomial(new[]
                {
                    2451900.05952,
                    365242.74049,
                    -0.06223,
                    -0.00823,
                    0.00032
                }, Y),
            _ => throw new ArgumentOutOfRangeException(nameof(markerNumber), "Invalid value."),
        };
    }

    /// <summary>
    /// Calculate approximate datetime of a seasonal marker.
    /// Algorithm is from AA2 Ch27 Equinoxes and Solstices (pp177-178).
    /// </summary>
    /// <param name="year">The year (-1000..3000)</param>
    /// <param name="markerNumber">The marker number (as enum)</param>
    /// <returns>The result in universal time.</returns>
    public static DateTime CalcSeasonalMarkerApprox(int year, ESeasonalMarker markerNumber)
    {
        double JDE0 = CalcSeasonalMarkerMean(year, markerNumber);
        double T = Terran.JulianCenturiesSinceJ2000(JDE0);
        double W = Angle.DegToRad(35999.373 * T - 2.47);
        double dLambda = 1 + 0.0334 * Cos(W) + 0.0007 * Cos(2 * W);

        // Sum the periodic terms from Table 27.C.
        List<(double A, double B, double C)> terms = SeasonalMarker.PeriodicTerms();
        double S = terms.Sum(term => term.A * Cos(Angle.DegToRad(term.B + term.C * T)));

        // Equation from p178.
        double jdtt = JDE0 + (0.00001 * S) / dLambda;

        // Get the date in Terrestrial Time (TT).
        DateTime dttt = XDateTime.FromJulianDay(jdtt);

        // Subtract ∆T to get Universal Time.
        long deltaT_ticks = (long)(CalcDeltaT(dttt) / XTimeSpan.SecondsPerTick);
        DateTime UT = dttt.Subtract(new TimeSpan(deltaT_ticks));
        return UT;
    }

    /// <summary>
    /// Higher-accuracy method for calculating seasonal marker.
    /// Algorithm is from AA2 p180.
    /// </summary>
    /// <param name="year">The year (-1000..3000)</param>
    /// <param name="markerNumber">The marker number (use enum)</param>
    /// <returns>The result in dynamical time.</returns>
    public static DateTime CalcSeasonalMarker(int year, ESeasonalMarker markerNumber)
    {
        double jd = CalcSeasonalMarkerMean(year, markerNumber);
        double k = (int)markerNumber;
        double targetLs = k * PI / 2;
        bool done;
        const double delta = 1E-9;
        do
        {
            (double Bs, double Ls) = Solar.CalcPosition(jd);
            double diffLs = targetLs - Ls;
            done = Abs(diffLs) < delta;
            if (!done)
            {
                double correction = 58 * Sin(diffLs);
                jd += correction;
            }
        } while (!done);

        return XDateTime.FromJulianDay(jd);
    }
}
