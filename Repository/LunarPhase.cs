namespace AstroMultimedia.Astronomy.Repository;

public enum ELunarPhase
{
    NewMoon = 0,
    FirstQuarter = 1,
    FullMoon = 2,
    ThirdQuarter = 3
}

public class LunarPhase
{
    #region Properties

    public int Id { get; set; }

    /// <summary>
    /// This value is:
    ///   0 = new moon
    ///   1 = first quarter
    ///   2 = full moon
    ///   3 = third quarter
    /// </summary>
    [Column(TypeName = "tinyint")]
    public int PhaseNumber { get; set; }

    /// <summary>
    /// The UTC datetime of the lunar phase.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime UtcDateTime { get; set; }

    #endregion Properties

    /// <summary>
    /// Extract the lunar phase data from the web pages captured from
    /// AstroPixels and copy the data into the database.
    /// </summary>
    public static void ParseLunarPhaseData()
    {
        Regex rxDataLine =
            new(@"(\d{4})?\s+(([a-z]{3})\s+(\d{1,2})\s+(\d{2}):(\d{2})\s+([a-z])?){1,4}",
                RegexOptions.IgnoreCase);
        int? year = null;
        string? curYearStr = null;

        using AstroDbContext db = new();

        for (int century = 0; century < 20; century++)
        {
            int startYear = century * 100 + 1;
            int endYear = (century + 1) * 100;
            string[] lines = File.ReadAllLines(
                $"{AstroDbContext.DataDirectory()}/Lunar phases/AstroPixels - Moon Phases_ {startYear:D4} to {endYear:D4}.html");
            foreach (string line in lines)
            {
                MatchCollection matches = rxDataLine.Matches(line);
                if (matches.Count <= 0)
                {
                    continue;
                }

                // Get the year, if present.
                string yearStr = line.Substring(1, 4);
                if (yearStr.Trim() != "")
                {
                    curYearStr = yearStr;
                    year = int.Parse(yearStr);
                }

                // The dates are meaningless without the year. This
                // shouldn't happen, but if we don't know the year, skip
                // parsing the dates.
                if (year == null)
                {
                    continue;
                }

                // Get the dates and times of the phases, if present.
                for (int phase = 0; phase < 4; phase++)
                {
                    string dateStr = line.Substring(8 + (phase * 18), 13);
                    if (dateStr.Trim() == "")
                    {
                        continue;
                    }
                    bool parseOk = DateTime.TryParse($"{curYearStr} {dateStr}",
                        out DateTime phaseDateTime);
                    if (parseOk)
                    {
                        // Set the datetime to UTC.
                        phaseDateTime = DateTime.SpecifyKind(phaseDateTime, DateTimeKind.Utc);

                        // Store phase information in the database.
                        if (db.LunarPhases != null)
                        {
                            db.LunarPhases.Add(new LunarPhase
                            {
                                PhaseNumber = phase,
                                UtcDateTime = phaseDateTime
                            });
                            db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
