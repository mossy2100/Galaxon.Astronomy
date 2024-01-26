using Galaxon.Astronomy.Database;

namespace Galaxon.Astronomy.Models;

public class Planet : AstroObject
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public Planet() { }

    /// <summary>
    /// Load a planet from the database by name.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Planet? Load(AstroDbContext db, string name)
    {
        return Load(db.Planets, name);
    }

    /// <summary>
    /// Load a planet from the database by number.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Planet? Load(AstroDbContext db, uint num)
    {
        return Load(db.Planets, num);
    }

    /// <summary>
    /// I'll hard code this for now. If we find any new planets (or Ceres or Pluto are reclassified
    /// again, haha), we can update it.
    /// </summary>
    public const byte Count = 8;

    /// <summary>
    /// Get all the planets. There are only 8, so this is a useful and efficient cache.
    /// </summary>
    private static List<Planet> _Cache = [];

    public static List<Planet> All
    {
        get
        {
            // If we didn't load them yet, do it now.
            if (_Cache.Count == 0)
            {
                using AstroDbContext db = new ();
                _Cache = db.Planets.ToList();
            }
            return _Cache;
        }
    }
}
