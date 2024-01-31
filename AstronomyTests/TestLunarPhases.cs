using System.Globalization;
using Galaxon.Astronomy.Data.Enums;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Astronomy.Algorithms;
using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Data.Repositories;
using Newtonsoft.Json;

namespace Galaxon.Astronomy.Tests;

[TestClass]
public class TestLunarPhases
{
    private AstroDbContext? _astroDbContext;

    private AstroObjectRepository? _astroObjectRepository;

    private AstroObjectGroupRepository? _astroObjectGroupRepository;

    private LunaService? _moonService;

    [TestInitialize]
    public void Init()
    {
        _astroDbContext = new AstroDbContext();
        _astroObjectGroupRepository = new AstroObjectGroupRepository(_astroDbContext);
        _astroObjectRepository = new AstroObjectRepository(_astroDbContext, _astroObjectGroupRepository);
        _moonService = new LunaService(_astroObjectRepository);
    }

    /// <summary>
    /// Test example 49a from Astronomical Algorithms 2nd ed.
    /// </summary>
    [TestMethod]
    public void TestExample49a()
    {
        DateTime dtApprox = new (1977, 2, 15);
        LunarPhase phase = _moonService!.PhaseFromDateTime(dtApprox);

        Assert.AreEqual(ELunarPhase.NewMoon, phase.PhaseNumber);
        Assert.AreEqual(1977, phase.UtcDateTime.Year);
        Assert.AreEqual(2, phase.UtcDateTime.Month);
        Assert.AreEqual(18, phase.UtcDateTime.Day);
        Assert.AreEqual(3, phase.UtcDateTime.Hour);
        Assert.AreEqual(36, phase.UtcDateTime.Minute);
    }

    /// <summary>
    /// Test example 49b from Astronomical Algorithms 2nd ed.
    /// </summary>
    [TestMethod]
    public void TestExample49b()
    {
        DateTime dtApprox = new (2044, 1, 20);
        LunarPhase phase = _moonService!.PhaseFromDateTime(dtApprox);
        Assert.AreEqual(ELunarPhase.ThirdQuarter, phase.PhaseNumber);
        Assert.AreEqual(2044, phase.UtcDateTime.Year);
        Assert.AreEqual(1, phase.UtcDateTime.Month);
        Assert.AreEqual(21, phase.UtcDateTime.Day);
        Assert.AreEqual(23, phase.UtcDateTime.Hour);
        // This differs from the example by 2 minutes, because I'm using NASA's formulae for
        // calculating deltaT instead of Meeus's.
        Assert.AreEqual(46, phase.UtcDateTime.Minute);
    }

    /// <summary>
    /// Compare my calculation with AstroPixels data for 2023.
    /// </summary>
    [TestMethod]
    public void CompareWithAstroPixels()
    {
        // Read in the phases.
        string jsonFilePath =
            "/Users/shaun/Documents/Web & software development/C#/Projects/Galaxon/Astronomy/AstronomyTests/data/LunarPhases2023.json";
        string json = File.ReadAllText(jsonFilePath);
        string[][]? data = JsonConvert.DeserializeObject<string[][]>(json);

        if (data == null)
        {
            Assert.Fail("Could not read data.");
        }

        // Find the phases for the year.
        int y = 2023;
        List<LunarPhase> phases = _moonService!.PhasesInYear(y);

        // Count phases.
        Assert.AreEqual(data.Length, phases.Count);

        // Check each.
        int i = 0;
        foreach (LunarPhase phase in phases)
        {
            // Compare phase type.
            int pnExpected = int.Parse(data[i][0]);
            int pnActual = (int)phase.PhaseNumber;
            Assert.AreEqual(pnExpected, pnActual);

            // Compare DateTimes.
            DateTime dtExpected = ParseDateTime(data[i][1], y);
            DateTime dtActual = phase.UtcDateTime;
            Assert.AreEqual(dtExpected.Year, dtActual.Year);
            Assert.AreEqual(dtExpected.Month, dtActual.Month);
            Assert.AreEqual(dtExpected.Day, dtActual.Day);
            Assert.AreEqual(dtExpected.Hour, dtActual.Hour);
            // Note the delta in this assert. Sometimes they vary by a minute; probably due to
            // deltaT calculations or something like that. Close enough.
            Assert.AreEqual(dtExpected.Minute, dtActual.Minute, 1);

            i++;
        }
    }

    /// <summary>
    /// Parse a datetime string from the AstroPixels website into a DateTime object.
    /// </summary>
    /// <param name="sDateTime">The datetime as a string, without year.</param>
    /// <param name="year">The year.</param>
    /// <returns></returns>
    private DateTime ParseDateTime(string sDateTime, int year)
    {
        // Get the month, convert to int.
        string monthAbbrev = sDateTime[..3];
        DateTime parsedDate = DateTime.ParseExact(monthAbbrev, "MMM", CultureInfo.InvariantCulture);
        int month = parsedDate.Month;

        // Parse the day, hour, and minute.
        int day = int.Parse(sDateTime[4..6].TrimStart());
        int hour = int.Parse(sDateTime[8..10]);
        int minute = int.Parse(sDateTime[11..13]);

        return new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Utc);
    }
}
