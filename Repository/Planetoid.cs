using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Repository;

/// <summary>
/// This class is for all dwarf planets and small Solar System bodies (i.e.
/// minor planets and comets).
/// All objects tracked by the Minor Planet Center other than satellites.
/// </summary>
public class Planetoid : AstroObject
{
    /// <summary>
    /// Check for case-insensitive match on name, number, packed designation, or
    /// readable designation.
    /// Only matches full string only (with whitespace trimmed).
    /// </summary>
    /// <param name="searchString">The search string.</param>
    /// <returns>If there's a match.</returns>
    /// TODO Test.
    public bool IsMatch(string searchString)
    {
        // Get the Number and Name together, without parentheses, which is
        // commonly used. e.g. "1 Ceres"
        string numberAndName = $"{Number} {Name}".Trim();
        return searchString.EqualsIgnoreCase(Name)
            || searchString == Number.ToString()
            || searchString == numberAndName
            || searchString == (MinorPlanetRecord?.ReadableDesignation ?? "")
            || searchString == (MinorPlanetRecord?.PackedDesignation ?? "");
    }

    /// <summary>
    /// Load a minor planet from the database.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name">This can be the minor planet or comet's:
    ///   * Name (e.g. "Ceres")
    ///   * Packed designation (e.g. "00001")
    ///   * Readable designation as shown in the Minor Planet Centre record
    ///     (e.g. "(1) Ceres").
    /// </param>
    /// <returns>The matching Planetoid object or null if not found.</returns>
    /// <exception cref="ArgumentNullException">If the search string == null or
    /// empty.</exception>
    /// TODO Test.
    public static Planetoid? Load(AstroDbContext db, string name)
    {
        // Search by name as usual.
        Planetoid? p = Load(db.Planetoids, name);
        if (p != null)
        {
            return p;
        }

        // Search for match on packed or readable designation.
        MinorPlanetRecord? mpr = db.MinorPlanetRecords
            .FirstOrDefault(mpr =>
                mpr.ReadableDesignation == name
                || mpr.PackedDesignation == name);

        // If not found, give up.
        return mpr == null ? null : db.Planetoids.Find(mpr.AstroObjectId);
    }

    public static Planetoid? Load(AstroDbContext db, uint num)
    {
        return Load(db.Planetoids, num);
    }
}
