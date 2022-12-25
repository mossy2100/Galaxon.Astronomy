namespace Galaxon.Astronomy.Repository;

public class DeltaTEntry
{
    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "smallint")]
    public int Year { get; set; }

    public double DeltaT { get; set; }

    #endregion Properties

    /// <summary>
    /// Extract values from the ∆T data file and transfer them to the
    /// database.
    /// The data file was created using OCR on the table, which was only
    /// available as an image.
    /// </summary>
    private static void ParseMeeusDeltaTTable()
    {
        using AstroDbContext db = new();

        string dataFile = $"{AstroDbContext.DataDirectory()}/DeltaT/Meeus AA Table 10A.txt";
        using StreamReader reader = new(dataFile);

        for (int group = 0; group < 5; group++)
        {
            // Read 3 lines from the text file.
            string? years = reader.ReadLine();
            string? deltaTValues = reader.ReadLine();
            _ = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(years) || string.IsNullOrWhiteSpace(deltaTValues))
            {
                continue;
            }

            int nEntries = (int)Ceiling(years.Length / 5.0);
            for (int entry = 0; entry < nEntries; entry++)
            {
                // Get the year and ∆T value.
                int year = int.Parse(years.Substring(entry * 5, 4).Trim());
                string deltaTString = entry == nEntries - 1
                    ? deltaTValues[(entry * 5)..]
                    : deltaTValues.Substring(entry * 5, 4);
                double deltaT = double.Parse(deltaTString.Trim());

                // Check if we already added the value for this year.
                DeltaTEntry? existingDeltaTEntry = db.DeltaTEntries
                    .FirstOrDefault(entry2 => entry2.Year == year);
                if (existingDeltaTEntry == null)
                {
                    // Add a new value.
                    db.DeltaTEntries.Add(new DeltaTEntry
                    {
                        Year = year,
                        DeltaT = deltaT
                    });
                }
                else
                {
                    // Update the existing value.
                    existingDeltaTEntry.DeltaT = deltaT;
                }
            } // for
        } // for

        db.SaveChanges();
    }
}
