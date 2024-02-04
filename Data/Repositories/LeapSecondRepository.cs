using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Time;
using HtmlAgilityPack;

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
        _list ??= astroDbContext.LeapSeconds.OrderBy(ls => ls.LeapSecondDate).ToList();

    /// <summary>
    /// NIST web page showing a table of leap seconds.
    /// An alternate source could be <see href="https://en.wikipedia.org/wiki/Leap_second"/> but I
    /// expect NIST is more likely to be correct and maintained up to date (but I could be wrong).
    /// As yet I haven't found a more authoritative source of leap seconds online. The IERS
    /// bulletins don't cover the entire period from 1972.
    /// </summary>
    private const string _NIST_LEAP_SECONDS_URL =
        "https://www.nist.gov/pml/time-and-frequency-division/time-realization/leap-seconds";

    /// <summary>
    /// Download the leap seconds from the NIST website.
    /// </summary>
    public async Task ParseNistWebPage()
    {
        using var httpClient = new HttpClient();

        // Load the HTML table as a string.
        // Fetch HTML content from the bulletins URL
        string htmlContent = await httpClient.GetStringAsync(_NIST_LEAP_SECONDS_URL);

        // Parse HTML content using HtmlAgilityPack.
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);

        // Get the tables.
        HtmlNodeCollection? tables = htmlDocument.DocumentNode.SelectNodes("//table");
        if (tables == null || tables.Count == 0)
        {
            throw new InvalidDataException("No tables found.");
        }

        // Get the table cells.
        HtmlNode? leapSecondTable = tables[0];
        HtmlNodeCollection? cells = leapSecondTable.SelectNodes(".//td");
        if (cells == null || cells.Count == 0)
        {
            throw new InvalidDataException("No table data found.");
        }

        // Loop through the cells looking for dates.
        List<DateOnly> dates = new ();
        Regex rxDate = new (@"^(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})$");
        foreach (HtmlNode? cell in cells)
        {
            Console.WriteLine(cell.InnerText.Trim());
            MatchCollection matches = rxDate.Matches(cell.InnerText.Trim());
            if (matches.Count == 1)
            {
                var year = int.Parse(matches[0].Groups["year"].Value);
                var month = int.Parse(matches[0].Groups["month"].Value);
                var day = int.Parse(matches[0].Groups["day"].Value);
                DateOnly date = new (year, month, day);
                dates.Add(date);
            }
        }

        // Sort, then add any new ones to database.
        if (dates.Count > 0)
        {
            foreach (DateOnly d in dates.OrderBy(d => d.GetTotalDays()))
            {
                LeapSecond? leapSecond =
                    astroDbContext.LeapSeconds.FirstOrDefault(ls => ls.LeapSecondDate == d);
                if (leapSecond == null)
                {
                    // Add the new record.
                    leapSecond = new LeapSecond();
                    leapSecond.LeapSecondDate = d;
                    // All the leap seconds in that table are positive.
                    leapSecond.Value = 1;
                    astroDbContext.LeapSeconds.Add(leapSecond);
                    await astroDbContext.SaveChangesAsync();
                }
            }
        }
    }

    /// <summary>
    /// URL of the IERS Bulletin C index.
    /// </summary>
    private const string _BULLETIN_INDEX_URL =
        "https://datacenter.iers.org/products/eop/bulletinc/";

    /// <summary>
    /// Every 6 months, scrape the IERS bulletins at
    /// <see href="https://datacenter.iers.org/products/eop/bulletinc/"/>
    /// and parse them to get the latest leap second dates.
    /// Compare data with table here:
    /// <see href="https://www.nist.gov/pml/time-and-frequency-division/time-realization/leap-seconds"/>
    /// </summary>
    /// <remarks>
    /// TODO Set up the cron job to check the IERS website periodically.
    /// </remarks>
    public async Task ImportIersBulletins()
    {
        using var httpClient = new HttpClient();

        try
        {
            // Fetch HTML content from the bulletins URL
            string htmlContent = await httpClient.GetStringAsync(_BULLETIN_INDEX_URL);

            // Parse HTML content using HtmlAgilityPack.
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            // Extract individual bulletin URLs
            var bulletinUrls = new List<string>();
            HtmlNodeCollection? bulletinNodes =
                htmlDocument.DocumentNode.SelectNodes("//a[contains(@href, 'bulletinc-')]");
            if (bulletinNodes != null)
            {
                foreach (HtmlNode? node in bulletinNodes)
                {
                    string href = node.GetAttributeValue("href", "");
                    string bulletinUrl = new Uri(new Uri(_BULLETIN_INDEX_URL), href).AbsoluteUri;
                    bulletinUrls.Add(bulletinUrl);
                }
            }

            // Regular expression to match any of the "no leap second" phrases.
            var rxNoLeapSecond = new Regex("(NO|No) (positive )?leap second will be introduced");
            var months = string.Join('|', XGregorianCalendar.MonthNames.Values);
            var rxLeapSecond = new Regex(
                $@"A (?<sign>positive|negative) leap second will be introduced at the end of (?<month>{months}) (?<year>\d{{4}}).");

            // Loop through individual bulletin URLs and process them
            foreach (string bulletinUrl in bulletinUrls)
            {
                Console.WriteLine("Bulletin URL: " + bulletinUrl);

                // Get the bulletin number.
                int bulletinNumber;
                string pattern = @"bulletinc-(\d+)\.txt$";
                Match match = Regex.Match(bulletinUrl, pattern);
                if (match.Success)
                {
                    bulletinNumber = int.Parse(match.Groups[1].Value);
                    Console.WriteLine($"Bulletin C number: {bulletinNumber}");
                }
                else
                {
                    throw new InvalidOperationException(
                        "PARSE ERROR: Could not get bulletin number.");
                }

                // Ignore Bulletin C 10.
                if (bulletinNumber == 10)
                {
                    Console.WriteLine($"Ignoring bulletin {bulletinNumber}.");
                    continue;
                }

                // Get the existing record if there is one.
                IersBulletinC? iersBulletinC = astroDbContext.IersBulletinCs.FirstOrDefault(
                    ls => ls.BulletinNumber == bulletinNumber);
                if (iersBulletinC == null)
                {
                    Console.WriteLine("Existing leap second record not found.");
                    iersBulletinC = new IersBulletinC();
                    astroDbContext.IersBulletinCs.Add(iersBulletinC);
                }
                else
                {
                    Console.WriteLine("Existing leap second record found.");
                    continue;
                }

                // Update leap second record.
                iersBulletinC.BulletinNumber = bulletinNumber;
                iersBulletinC.BulletinUrl = bulletinUrl;
                iersBulletinC.DateTimeParsed = DateTime.Now;

                // Parse content of bulletin.
                string bulletinText = await httpClient.GetStringAsync(bulletinUrl);
                if (rxNoLeapSecond.IsMatch(bulletinText))
                {
                    iersBulletinC.Value = 0;
                    iersBulletinC.LeapSecondDate = null;
                    Console.WriteLine("No leap second.");
                }
                else
                {
                    MatchCollection matches = rxLeapSecond.Matches(bulletinText);
                    if (matches.Count == 0)
                    {
                        throw new InvalidOperationException(
                            "PARSE ERROR: Could not detect leap second value.");
                    }

                    GroupCollection groups = matches[0].Groups;
                    iersBulletinC.Value = (sbyte)(groups["sign"].Value == "positive" ? 1 : -1);
                    int month = XGregorianCalendar.MonthNameToNumber(groups["month"].Value);
                    int year = int.Parse(groups["year"].Value);
                    iersBulletinC.LeapSecondDate = XGregorianCalendar.MonthLastDay(year, month);

                    // Update or insert the leap second record.
                    LeapSecond? leapSecond = astroDbContext.LeapSeconds.FirstOrDefault(ls =>
                        ls.LeapSecondDate == iersBulletinC.LeapSecondDate);
                    if (leapSecond == null)
                    {
                        // Create new record.
                        leapSecond = new LeapSecond();
                        leapSecond.LeapSecondDate = iersBulletinC.LeapSecondDate.Value;
                        leapSecond.Value = iersBulletinC.Value;
                        astroDbContext.LeapSeconds.Add(leapSecond);
                    }
                }
                Console.WriteLine($"Value = {iersBulletinC.Value}");
                Console.WriteLine($"Leap second date = {iersBulletinC.LeapSecondDate}");

                // Update or insert the record.
                await astroDbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }
    }
}
