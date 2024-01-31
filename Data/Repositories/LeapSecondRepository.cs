using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Data.Repositories;

public class LeapSecondRepository(AstroDbContext astroDbContext)
{
    /// <summary>
    /// Cache of the leap seconds so we don't have to keep loading them if they're needed more than
    /// once during a program.
    /// </summary>
    private List<LeapSecond>? _list;

    /// <summary>
    /// Get all leap seconds inserted so far, in date order.
    /// </summary>
    public List<LeapSecond> List =>
        _list ??= astroDbContext.LeapSeconds.Where(ls => ls.Value != 0 && ls.LeapSecondDate != null)
            .OrderBy(ls => ls.LeapSecondDate).ToList();

    #region Parsing and importing

    /// <summary>
    /// Initialise the data table with the current dates that had leap seconds.
    /// </summary>
    public void ParseLeapSeconds()
    {
        // Load the HTML table as a string.
        var dataFilePath =
            $"{AstroDbContext.DataDirectory()}/Leap seconds/Leap second and UT1-UTC information _ NIST.html";
        string html = File.ReadAllText(dataFilePath);

        // Get the dates.
        Regex rxDate = new (@"(\d{4})-(\d{2})-(\d{2})");
        MatchCollection matches = rxDate.Matches(html);
        List<DateOnly> dates = new ();
        foreach (Match match in matches)
        {
            var year = int.Parse(match.Groups[1].Value);
            var month = int.Parse(match.Groups[2].Value);
            var day = int.Parse(match.Groups[3].Value);
            DateOnly date = new (year, month, day);
            dates.Add(date);
        }

        // Sort, then add to database.
        if (dates.Count > 0)
        {
            foreach (DateOnly dt in dates.OrderBy(d => d.GetTotalDays()))
            {
                LeapSecond? existingRecord =
                    astroDbContext.LeapSeconds.FirstOrDefault(ls => ls.LeapSecondDate == dt);
                if (existingRecord == null)
                {
                    // Add a new record.
                    astroDbContext.LeapSeconds.Add(new LeapSecond { LeapSecondDate = dt });
                    astroDbContext.SaveChanges();
                }
            }
        }
    }

    /// <summary>
    /// Every 6 months, scrape the IERS bulletins at
    /// <see href="https://datacenter.iers.org/products/eop/bulletinc/"/>
    /// and parse them to get the latest leap second dates.
    ///
    /// TODO
    /// 1. Set up the database.
    /// 2. Write the code to import and parse the bulletins.
    /// 2. Set up the cron job to check the IERS website periodically.
    /// </summary>
    public static void ImportLeapSeconds() { }

    #endregion Parsing and importing
}
