using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Astronomy.Data.Enums;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Exceptions;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Algorithms;

/// <summary>
/// Calling this class LunaService as there was some earlier confusion with the word "moon"
/// referring to "the Moon" as well as natural satellites.
/// To avoid confusion the code now refers to the Moon as "Luna" and natural satellites as
/// "satellites".
/// </summary>
/// <param name="astroObjectRepository"></param>
public class LunaService(AstroObjectRepository astroObjectRepository)
{
    /// <summary>
    /// Start point of Meeus' lunation number (LN) 0.
    /// </summary>
    public static DateTime LUNATION_0_START { get; } =
        new (2000, 1, 6, 18, 14, 0, DateTimeKind.Utc);

    /// <summary>
    /// Cached reference to the AstroObject representing Luna.
    /// </summary>
    private AstroObject? _luna;

    /// <summary>
    /// Get the AstroObject representing Luna.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="DataNotFoundException"></exception>
    public AstroObject GetPlanet()
    {
        if (_luna == null)
        {
            AstroObject? luna = astroObjectRepository.Load("Luna", "planet");
            _luna = luna
                ?? throw new DataNotFoundException("Could not find the Moon (Luna) in the database.");
        }

        return _luna;
    }

    /// <summary>
    /// Find the DateTime (UTC) of the next specified lunar phase on or after the given DateTime.
    /// Algorithm taken from Chapter 49 "Phases of the Moon", Astronomical Algorithms 2nd ed. by
    /// Jean Meeus (1998).
    /// </summary>
    /// <param name="dt">Approximate DateTime of the phase.</param>
    /// <returns>
    /// A LunarPhase object, which contains information about which phase it is, and the approximate
    /// datetime of the event.
    /// </returns>
    public LunarPhase PhaseFromDateTime(DateTime dt)
    {
        // Calculate k, rounded off to nearest 0.25.
        double k = (dt - LUNATION_0_START).Ticks / XTimeSpan.TICKS_PER_LUNATION;
        int phaseNumber = (int)Round(k * 4.0);
        k = phaseNumber / 4.0;

        // Calculate T and powers of T.
        // I might have overdone it here on attempting to maximise precision for this calculation.
        // Testing will confirm.
        DateTime dtPhaseApprox = LUNATION_0_START.AddDays(k * XTimeSpan.DAYS_PER_LUNATION);
        double JD = dtPhaseApprox.ToJulianDate();
        double JDTT = TimeScaleService.JulianDateUniversalToTerrestrial(JD);
        double T = TimeScaleService.JulianCenturiesSinceJ2000(JDTT);
        double T2 = T * T;
        double T3 = T * T2;
        double T4 = T * T3;

        // Calculate JDE.
        double JDE = 2_451_550.097_66 + 29.530_588_861 * k + 0.000_154_37 * T2 + 0.000_000_150 * T3
            + 0.000_000_000_73 * T4;

        // Calculate E.
        double E = 1 - 0.002_516 * T - 0.000_0074 * T2;
        double E2 = E * E;

        // Local function to convert angles to radians in the range [0..tau).

        // Calculate Sun's mean anomaly at time JDE (radians).
        double M = Deg2Rad(2.5534 + 29.105_356_70 * k - 0.000_001_4 * T2
            - 0.000_000_11 * T3);

        // Calculate Luna's mean anomaly at time JDE (radians).
        double L = Deg2Rad(201.5643 + 385.816_935_28 * k + 0.010_758_2 * T2
            + 0.000_012_38 * T3 - 0.000_000_058 * T4);

        // Calculate Luna's argument of latitude (radians).
        double F = Deg2Rad(160.710_8 + 390.670_502_84 * k - 0.001_6118 * T2
            - 0.000_002_27 * T2 + 0.000_000_011 * T4);

        // Calculate the longitude of the ascending node of the lunar orbit (radians).
        double Omega = Deg2Rad(124.7746 - 1.563_755_88 * k + 0.002_0672 * T2
            + 0.000_002_15 * T3);

        // Calculate planetary arguments (radians).
        double A1 = Deg2Rad(299.77 + 0.107_408 * k - 0.009_173 * T2);
        double A2 = Deg2Rad(251.88 + 0.016_321 * k);
        double A3 = Deg2Rad(251.83 + 26.651_886 * k);
        double A4 = Deg2Rad(349.42 + 36.412_478 * k);
        double A5 = Deg2Rad(84.66 + 18.206_239 * k);
        double A6 = Deg2Rad(141.74 + 53.303_771 * k);
        double A7 = Deg2Rad(207.14 + 2.453_732 * k);
        double A8 = Deg2Rad(154.84 + 7.306_860 * k);
        double A9 = Deg2Rad(34.52 + 27.261_239 * k);
        double A10 = Deg2Rad(207.19 + 0.121_824 * k);
        double A11 = Deg2Rad(291.34 + 1.844_379 * k);
        double A12 = Deg2Rad(161.72 + 24.198_154 * k);
        double A13 = Deg2Rad(239.56 + 25.513_099 * k);
        double A14 = Deg2Rad(331.55 + 3.592_518 * k);

        ELunarPhase phaseType = (ELunarPhase)(phaseNumber % 4);
        double C1;
        if (phaseType is ELunarPhase.NewMoon or ELunarPhase.FullMoon)
        {
            if (phaseType == ELunarPhase.NewMoon)
            {
                // Phase type is New Moon.
                C1 = -0.40720 * Sin(L)
                    + 0.17241 * E * Sin(M)
                    + 0.01608 * Sin(2 * L)
                    + 0.01039 * Sin(2 * F)
                    + 0.00739 * E * Sin(L - M)
                    - 0.00514 * E * Sin(L + M)
                    + 0.00208 * E2 * Sin(2 * M);
            }
            else
            {
                // Phase type is Full Moon.
                C1 = -0.40614 * Sin(L)
                    + 0.17302 * E * Sin(M)
                    + 0.01614 * Sin(2 * L)
                    + 0.01043 * Sin(2 * F)
                    + 0.00734 * E * Sin(L - M)
                    - 0.00515 * E * Sin(L + M)
                    + 0.00209 * E2 * Sin(2 * M);
            }

            // Phase type is New Moon or Full Moon.
            C1 += -0.00111 * Sin(L - 2 * F)
                - 0.00057 * Sin(L + 2 * F)
                + 0.00056 * E * Sin(2 * L + M)
                - 0.00042 * Sin(3 * L)
                + 0.00042 * E * Sin(M + 2 * F)
                + 0.00038 * E * Sin(M - 2 * F)
                - 0.00024 * E * Sin(2 * L - M)
                - 0.00017 * Sin(Omega)
                - 0.00007 * Sin(L + 2 * M)
                + 0.00004 * Sin(2 * (L - F))
                + 0.00004 * Sin(3 * M)
                + 0.00003 * Sin(L + M - 2 * F)
                + 0.00003 * Sin(2 * (L + F))
                - 0.00003 * Sin(L + M + 2 * F)
                + 0.00003 * Sin(L - M + 2 * F)
                - 0.00002 * Sin(L - M - 2 * F)
                - 0.00002 * Sin(3 * L + M)
                + 0.00002 * Sin(4 * L);
        }
        else
        {
            // Phase type is First Quarter or Third Quarter.
            C1 = -0.62801 * Sin(L)
                + 0.17172 * E * Sin(M)
                - 0.01183 * E * Sin(L + M)
                + 0.00862 * Sin(2 * L)
                + 0.00804 * Sin(2 * F)
                + 0.00454 * E * Sin(L - M)
                + 0.00204 * E2 * Sin(2 * M)
                - 0.00180 * Sin(L - 2 * F)
                - 0.00070 * Sin(L + 2 * F)
                - 0.00040 * Sin(3 * L)
                - 0.00034 * E * Sin(2 * L - M)
                + 0.00032 * E * Sin(M + 2 * F)
                + 0.00032 * E * Sin(M - 2 * F)
                - 0.00028 * E2 * Sin(L + 2 * M)
                + 0.00027 * E * Sin(2 * L + M)
                - 0.00017 * Sin(Omega)
                - 0.00005 * Sin(L - M - 2 * F)
                + 0.00004 * Sin(2 * (L + F))
                - 0.00004 * Sin(L + M + 2 * F)
                + 0.00004 * Sin(L - 2 * M)
                + 0.00003 * Sin(L + M - 2 * F)
                + 0.00003 * Sin(3 * M)
                + 0.00002 * Sin(2 * (L - F))
                + 0.00002 * Sin(L - M + 2 * F)
                - 0.00002 * Sin(3 * L + M);

            // Calculate additional correction for first and third quarter phases.
            double W = 0.00306 - 0.00038 * E * Cos(M) + 0.00026 * Cos(L) - 0.00002 * Cos(L - M)
                + 0.00002 * Cos(L + M) + 0.00002 * Cos(2 * F);
            JDE += phaseType == ELunarPhase.FirstQuarter ? W : -W;
        }

        // Additional correction for all phases.
        double C2 = 0.000_325 * Sin(A1)
            + 0.000_165 * Sin(A2)
            + 0.000_164 * Sin(A3)
            + 0.000_126 * Sin(A4)
            + 0.000_110 * Sin(A5)
            + 0.000_062 * Sin(A6)
            + 0.000_060 * Sin(A7)
            + 0.000_056 * Sin(A8)
            + 0.000_047 * Sin(A9)
            + 0.000_042 * Sin(A10)
            + 0.000_040 * Sin(A11)
            + 0.000_037 * Sin(A12)
            + 0.000_035 * Sin(A13)
            + 0.000_023 * Sin(A14);

        // Apply corrections.
        JDE += C1 + C2;

        // Convert the JDE to a UTC DateTime.
        JD = TimeScaleService.JulianDateTerrestrialToUniversal(JDE);
        DateTime dtPhase = XDateTime.FromJulianDate(JD);

        // Construct and return the LunarPhase object.
        return new LunarPhase { PhaseNumber = phaseType, UtcDateTime = dtPhase };
    }

    private static double Deg2Rad(double deg)
    {
        return Angle.DegToRad(Angle.NormalizeDegrees(deg, false));
    }

    /// <summary>
    /// Get the DateTimes of all lunar phases in a given period.
    /// </summary>
    /// <param name="start">The start of the period.</param>
    /// <param name="end">The end of the period.</param>
    /// <returns></returns>
    public List<LunarPhase> PhasesInPeriod(DateTime start, DateTime end)
    {
        List<LunarPhase> result = [];

        // Find the phase nearest to start.
        LunarPhase phase = PhaseFromDateTime(start);

        // If it's in range, add it.
        if (phase.UtcDateTime >= start)
        {
            result.Add(phase);
        }

        // Get the average number of days between phases.
        const double daysPerPhase = XTimeSpan.DAYS_PER_LUNATION / 4.0;

        // Iteratively jump forward to the approximate moment of the next phase, and find the
        // exact moment of the phase, until we reach the finish DateTime.
        while (true)
        {
            // Get the next new moon in the series.
            DateTime nextGuess = phase.UtcDateTime.AddDays(daysPerPhase);
            phase = PhaseFromDateTime(nextGuess);

            // We done?
            if (phase.UtcDateTime > end)
            {
                break;
            }

            // Add it to the result.
            result.Add(phase);
        }

        return result;
    }

    /// <summary>
    /// Get the DateTimes of all occurrences of the specified phase in a given Gregorian calendar
    /// year.
    /// </summary>
    /// <param name="year">The year number.</param>
    /// <returns></returns>
    public List<LunarPhase> PhasesInYear(int year)
    {
        // Check year is valid. Valid range matches DateTime.IsLeapYear().
        if (year is < 1 or > 9999)
        {
            throw new ArgumentOutOfRangeException(nameof(year),
                "Year must be in the range 1..9999");
        }

        return PhasesInPeriod(XDateTime.GetYearStart(year), XDateTime.GetYearEnd(year));
    }
}
