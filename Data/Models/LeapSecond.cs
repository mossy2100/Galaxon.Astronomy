using Galaxon.Astronomy.Data;
using Galaxon.Core.Time;

namespace Galaxon.Astronomy.Models;

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
                using AstroDbContext db = new ();
                _list = db.LeapSeconds.ToList();
            }
            return _list;
        }
    }

    #endregion Properties
}
