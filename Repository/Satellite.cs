using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Repository;

public class Satellite : AstroObject
{
    /// <summary>
    /// Check for case-insensitive match on name.
    /// Only matches full string only (with whitespace trimmed).
    /// </summary>
    /// <param name="searchString">The search string.</param>
    /// <returns>If there's a match.</returns>
    /// TODO Test.
    public bool IsMatch(string searchString)
    {
        return searchString.EqualsIgnoreCase(Name);
    }

    /// <summary>
    /// Load a satellite from the database.
    /// </summary>
    /// <param name="name">The name of the satellite.</param>
    /// <returns>The Moon object.</returns>
    //public static Satellite? Load(AstroDbContext db, string name)
    //    => db.Satellites.FirstOrDefault(sat => sat.IsMatch(name));
    public static Satellite? Load(AstroDbContext db, string name)
    {
        return Load(db.Satellites, name);
    }

    public static Satellite? Load(AstroDbContext db, uint num)
    {
        return Load(db.Satellites, num);
    }
}
