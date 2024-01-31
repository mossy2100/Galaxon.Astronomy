using System.Data;
using Galaxon.Astronomy.Database;

namespace Galaxon.Astronomy.Data.Models;

public class Star : AstroObject
{
    /// <summary>
    /// Load a star from the database using its name.
    /// </summary>
    /// <param name="db">The DbContext.</param>
    /// <param name="name">The name of the star.</param>
    /// <returns>The Star object.</returns>
    /// <exception cref="DataException">If a matching record was not found.</exception>
    public static Star? Load(AstroDbContext db, string name)
    {
        return Load(db.Stars, name);
    }

    //public static Star? Load(AstroDbContext db, string searchString)
    //{
    //    return db.Stars
    //        .Include(star => star.Groups)
    //        .Include(star => star.Physical)
    //        .Include(star => star.Orbit)
    //        .Include(star => star.Rotation)
    //        .Include(star => star.Observation)
    //        .Include(star => star.Atmosphere)
    //        .Include(star => star.Stellar)
    //        .FirstOrDefault(star => star.Name == searchString);
    //}
}
