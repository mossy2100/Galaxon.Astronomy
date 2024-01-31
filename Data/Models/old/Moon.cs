using Galaxon.Astronomy.Database;
using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// Represents a planetary satellite.
/// </summary>
public class Moon : AstroObject
{
    /// <summary>
    /// Checks for a case-insensitive match on the moon's name.
    /// </summary>
    /// <param name="searchString">The search string to match against the moon's name.</param>
    /// <returns>True if there's a match; otherwise, false.</returns>
    public bool IsMatch(string searchString)
    {
        return searchString.EqualsIgnoreCase(Name);
    }

    /// <summary>
    /// Load a moon object from the database by name.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="name">The name of the moon to load.</param>
    /// <returns>The Moon object if found; otherwise, null.</returns>
    public static Moon? Load(AstroDbContext db, string name)
    {
        return Load(db.Moons, name);
    }

    /// <summary>
    /// Load a moon object from the database by its unique identifier.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="num">The unique identifier of the moon to load.</param>
    /// <returns>The Moon object if found; otherwise, null.</returns>
    public static Moon? Load(AstroDbContext db, uint num)
    {
        return Load(db.Moons, num);
    }
}
