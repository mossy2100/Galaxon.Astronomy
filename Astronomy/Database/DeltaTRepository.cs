using Galaxon.Astronomy.Models;

namespace Galaxon.Astronomy.Database;

public class DeltaTRepository
{
    /// <summary>
    /// Extract values from the ∆T data file and transfer them to the database.
    /// The data file was created using OCR on the table, which was only available as an image.
    /// </summary>
    private static void ParseMeeusDeltaTTable()
    {
        using AstroDbContext db = new ();

        var dataFile = $"{AstroDbContext.DataDirectory()}/DeltaT/Meeus AA Table 10A.txt";
        using StreamReader reader = new (dataFile);

        for (var group = 0; group < 5; group++)
        {
            // Read 3 lines from the text file.
            string? years = reader.ReadLine();
            string? deltaTValues = reader.ReadLine();
            _ = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(years) || string.IsNullOrWhiteSpace(deltaTValues))
            {
                continue;
            }

            var nEntries = (int)Ceiling(years.Length / 5.0);
            for (var entry = 0; entry < nEntries; entry++)
            {
                // Get the year and ∆T value.
                var year = int.Parse(years.Substring(entry * 5, 4).Trim());
                string deltaTString = entry == nEntries - 1
                    ? deltaTValues[(entry * 5)..]
                    : deltaTValues.Substring(entry * 5, 4);
                var deltaT = double.Parse(deltaTString.Trim());

                // Check if we already added the value for this year.
                DeltaTRecord? existingDeltaTEntry =
                    db.DeltaTRecords.FirstOrDefault(entry2 => entry2.Year == year);
                if (existingDeltaTEntry == null)
                {
                    // Add a new value.
                    db.DeltaTRecords.Add(new DeltaTRecord
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
