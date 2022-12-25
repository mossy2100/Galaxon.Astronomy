using Galaxon.Core.Time;
using Galaxon.Numerics.Maths;

namespace Galaxon.Astronomy.Repository;

public enum ESeasonalMarker
{
    MarchEquinox = 0,
    JuneSolstice = 1,
    SeptemberEquinox = 2,
    DecemberSolstice = 3
}

public class SeasonalMarker
{
    #region Properties

    public int Id { get; set; }

    /// <summary>
    /// This value is:
    ///   0 = March equinox
    ///   1 = June solstice
    ///   2 = September equinox
    ///   3 = December solstice
    /// </summary>
    [Column(TypeName = "tinyint")]
    public int MarkerNumber { get; set; }

    /// <summary>
    /// The UTC datetime of the seasonal marker.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime UtcDateTime { get; set; }

    #endregion Properties

    /// <summary>
    /// Extract the seasonal marker data from the text file and copy the data
    /// into the database.
    /// </summary>
    public static void ParseSeasonalMarkerData()
    {
        using AstroDbContext db = new();

        // Lookup table to help with the parsing.
        Dictionary<string, int> MonthAbbrevs = new()
        {
            { "Mar", 3 },
            { "Jun", 6 },
            { "Sep", 9 },
            { "Dec", 12 }
        };

        // Get the data from the data file as an array of strings.
        string dataFilePath =
            $"{AstroDbContext.DataDirectory()}/Seasonal markers/SeasonalMarkers2001-2100.txt";
        string[] lines = File.ReadAllLines(dataFilePath);

        // Convert the lines into our internal data structure.
        Regex rx = new("\\s+");
        foreach (string line in lines)
        {
            string[] words = rx.Split(line);

            if (words.Length <= 1 || words[0] == "Year")
            {
                continue;
            }

            // Extract the dates from the row.
            int year = int.Parse(words[1]);
            for (int i = 0; i < 4; i++)
            {
                int j = i * 3;
                string monthAbbrev = words[j + 2];
                int month = MonthAbbrevs[monthAbbrev];
                int dayOfMonth = int.Parse(words[j + 3]);
                string[] time = words[j + 4].Split(":");
                int hour = int.Parse(time[0]);
                int minute = int.Parse(time[1]);

                // Construct the new DateTime object.
                DateTime seasonalMarkerDateTime = new(year, month, dayOfMonth, hour, minute, 0,
                    DateTimeKind.Utc);

                // Check if there is already an entry in the database table
                // for this seasonal marker.
                SeasonalMarker? sm = db.SeasonalMarkers?
                    .FirstOrDefault(sm => sm.UtcDateTime.Year == year && sm.MarkerNumber == i);

                // Add a new row or update the existing row as required.
                if (sm == null)
                {
                    // Add a new row.
                    db.SeasonalMarkers!.Add(new SeasonalMarker
                    {
                        MarkerNumber = i,
                        UtcDateTime = seasonalMarkerDateTime
                    });
                }
                else
                {
                    // Update the row.
                    sm.UtcDateTime = seasonalMarkerDateTime;
                }
            }
        }

        db.SaveChanges();
    }

    /// <summary>
    /// Values from Table 27.C in Astronomical Algorithms 2nd ed.
    /// NB: B and C are in degrees.
    /// </summary>
    /// <returns></returns>
    public static List<(double A, double B, double C)> PeriodicTerms() =>
        new()
        {
            (485, 324.96, 1934.136),
            (203, 337.23, 32964.467),
            (199, 342.08, 20.186),
            (182, 27.85, 445267.112),
            (156, 73.14, 45036.886),
            (136, 171.52, 22518.443),
            (77, 222.54, 65928.934),
            (74, 296.72, 3034.906),
            (70, 243.58, 9037.513),
            (58, 119.81, 33718.147),
            (52, 297.17, 150.678),
            (50, 21.02, 2281.226),
            (45, 247.54, 29929.562),
            (44, 325.15, 31555.956),
            (29, 60.93, 4443.417),
            (18, 155.12, 67555.328),
            (17, 288.79, 4562.452),
            (16, 198.04, 62894.029),
            (14, 199.76, 31436.921),
            (12, 95.39, 14577.848),
            (12, 287.11, 31931.756),
            (12, 320.81, 34777.259),
            (9, 227.73, 1222.114),
            (8, 15.45, 16859.074)
        };
}
