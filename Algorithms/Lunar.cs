using Galaxon.Astronomy.Repository.Enums;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Algorithms;

public static class Lunar
{
    /// <summary>
    /// Find the DateTime (UTC) of the New Moon nearest to the given DateTime.
    /// </summary>
    /// <param name="phase">The lunar phase.</param>
    /// <param name="dt">Approximate DateTime of the phase.</param>
    /// <returns></returns>
    public static DateTime PhaseNearestToDateTime(ELunarPhase phase, DateTime dt)
    {
        return new DateTime();
    }

    /// <summary>
    /// Get the DateTimes of all New Moon phases in a given period.
    /// </summary>
    /// <param name="phase">The lunar phase.</param>
    /// <param name="start">The start of the period.</param>
    /// <param name="end">The end of the period.</param>
    /// <returns></returns>
    public static List<DateTime> PhasesInPeriod(ELunarPhase phase, DateTime start, DateTime end)
    {
        List<DateTime> result = new List<DateTime>();

        // 1. Find the new moon nearest to start.
        DateTime newMoon = PhaseNearestToDateTime(phase, start);
        DateTime nextGuess;
        if (newMoon >= start)
        {
            result.Add(newMoon);
        }

        // 2. Iteratively jump forward by an average lunation and look for the nearest new moon
        // until we go past the finish DateTime.
        while (true)
        {
            // Get the next new moon in the series.
            nextGuess = newMoon.AddDays(XTimeSpan.DAYS_PER_LUNATION);
            newMoon = PhaseNearestToDateTime(phase, nextGuess);

            // We done?
            if (newMoon > end)
            {
                break;
            }

            // Add it to the result.
            result.Add(newMoon);
        }

        return result;
    }

    /// <summary>
    /// Get the DateTimes of all New Moon phases in a given year (Gregorian).
    /// </summary>
    /// <param name="phase">The lunar phase.</param>
    /// <param name="y">The year number.</param>
    /// <returns></returns>
    public static List<DateTime> PhasesInYear(ELunarPhase phase, int y)
    {
        return PhasesInPeriod(phase, XDateTime.GetYearStart(y), XDateTime.GetYearEnd(y));
    }
}
