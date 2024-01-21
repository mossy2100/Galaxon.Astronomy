namespace Galaxon.Astronomy.Repository;

public class EasterDate
{
    /// <summary>
    /// Parse the data file from the US Census Bureau.
    /// </summary>
    /// <see href="https://www.census.gov/data/software/x13as/genhol/easter-dates.html"/>
    private static void ParseEasterDates1600_2099()
    {
        using AstroDbContext db = new ();

        var csvFile =
            $"{AstroDbContext.DataDirectory()}/Easter/Easter Sunday Dates 1600-2099.csv";
        using StreamReader reader = new (csvFile);

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            string[] values = line.Split(',');
            if (values.Length != 3)
            {
                continue;
            }

            try
            {
                var year = int.Parse(values[0]);
                var month = int.Parse(values[1]);
                var day = int.Parse(values[2]);
                DateOnly newEasterDate = new (year, month, day);

                // See if we already have one for this year.
                EasterDate? existingEasterDate = db.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);

                // Add or update the record as needed.
                if (existingEasterDate == null)
                {
                    // Add a record.
                    Console.WriteLine($"Adding new Easter date {newEasterDate}");
                    db.EasterDates.Add(new EasterDate { Date = newEasterDate });
                }
                else if (existingEasterDate.Date != newEasterDate)
                {
                    // Update the record.
                    Console.WriteLine(
                        $"Dates for {year} are not the same! Existing = {existingEasterDate.Date}, new = {newEasterDate}");
                    existingEasterDate.Date = newEasterDate;
                }
                else
                {
                    Console.WriteLine($"Dates for {year} are the same, nothing to do.");
                }
            }
            catch
            {
                Console.WriteLine($"Invalid line in CSV: {line}");
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Parse the data file from the Astronomical Society of South Australia.
    /// </summary>
    /// <see href="https://www.assa.org.au/edm"/>
    private static void ParseEasterDates1700_2299()
    {
        using AstroDbContext db = new ();

        var htmlFile =
            $"{AstroDbContext.DataDirectory()}/Easter/Easter Sunday Dates 1700-2299.html";
        Regex rx = new (@"(\d{1,2})(st|nd|rd|th) (March|April) (\d{4})");
        using StreamReader reader = new (htmlFile);

        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            MatchCollection matches = rx.Matches(line);
            if (matches.Count <= 0)
            {
                continue;
            }
            foreach (Match match in matches)
            {
                var year = int.Parse(match.Groups[4].Value);
                int month = match.Groups[3].Value == "March" ? 3 : 4;
                var day = int.Parse(match.Groups[1].Value);
                DateOnly newEasterDate = new (year, month, day);
                // See if we already have one for this year.
                EasterDate? existingEasterDate = db.EasterDates
                    .FirstOrDefault(ed => ed.Date.Year == year);
                if (existingEasterDate == null)
                {
                    db.EasterDates.Add(new EasterDate { Date = newEasterDate });
                }
                else
                {
                    // Check if they are different.
                    if (existingEasterDate.Date != newEasterDate)
                    {
                        Console.WriteLine(
                            $"Dates for {year} are not the same! Existing = {existingEasterDate.Date}, new = {newEasterDate}");
                        //existingEasterDate.Date = newEasterDate;
                    }
                    else
                    {
                        Console.WriteLine($"Dates for {year} are the same, nothing to do.");
                    }
                }
            }
        }

        db.SaveChanges();
    }

    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateOnly Date { get; set; }

    #endregion Properties
}
