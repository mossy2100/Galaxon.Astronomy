using AstroMultimedia.Core.Time;

namespace AstroMultimedia.Astronomy.Repository;

public class LeapSecond
{
    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateOnly Date { get; set; }

    /// <summary>
    /// Cache of the leap seconds so we don't have to keep loading them in the
    /// event they're needed more than once during a program.
    /// </summary>
    private static List<LeapSecond>? _list;

    public static List<LeapSecond> List
    {
        get
        {
            if (_list == null)
            {
                // Load the leap seconds from the database.
                using AstroDbContext db = new();
                _list = db.LeapSeconds.ToList();
            }
            return _list;
        }
    }

    #endregion Properties

    /// <summary>
    /// Initialise the data table with the current dates that had leap seconds.
    /// </summary>
    public static void ParseData()
    {
        using AstroDbContext db = new();

        // Load the HTML table as a string.
        string dataFilePath =
            $"{AstroDbContext.DataDirectory()}/Leap seconds/Leap second and UT1-UTC information _ NIST.html";
        string html = File.ReadAllText(dataFilePath);

        // Get the dates.
        Regex rxDate = new(@"(\d{4})-(\d{2})-(\d{2})");
        MatchCollection matches = rxDate.Matches(html);
        List<DateOnly> dates = new();
        foreach (Match match in matches)
        {
            int year = int.Parse(match.Groups[1].Value);
            int month = int.Parse(match.Groups[2].Value);
            int day = int.Parse(match.Groups[3].Value);
            DateOnly date = new(year, month, day);
            dates.Add(date);
        }

        // Sort, then add to database.
        if (dates.Count > 0)
        {
            foreach (DateOnly dt in dates.OrderBy(d => d.GetTotalDays()))
            {
                LeapSecond? existingRecord = db.LeapSeconds.FirstOrDefault(ls => ls.Date == dt);
                if (existingRecord == null)
                {
                    // Add a new record.
                    db.LeapSeconds.Add(new LeapSecond { Date = dt });
                    db.SaveChanges();
                }
            }
        }
    }
}
