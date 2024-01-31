using Galaxon.Astronomy.Database;
using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// This class is for all dwarf planets and small Solar System bodies (i.e. minor planets and
/// comets).
/// This includes all objects tracked by the Minor Planet Center other than satellites.
/// </summary>
public class Planetoid : AstroObject
{
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
    /// <exception cref="ArgumentNullException">
    /// If the search string is null or empty.
    /// </exception>
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
