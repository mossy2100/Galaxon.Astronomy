using Galaxon.Astronomy.Database;

namespace Galaxon.Astronomy.Models;

public class AstroObjectGroup
{
    // Primary key.
    public int Id { get; set; }

    // Group name.
    public string Name { get; set; } = "";

    // Objects in the group (navigation property).
    public List<AstroObject> Objects { get; set; } = new ();

    // Parent group.
    public int? ParentId { get; set; }

    public AstroObjectGroup? Parent { get; set; }

    /// <summary>
    /// Load an AstroObjectGroup from the database.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="searchString">The group name.</param>
    /// <returns>The found object or null.</returns>
    public static AstroObjectGroup? Load(AstroDbContext db, string searchString)
    {
        return db.AstroObjectGroups.FirstOrDefault(cat => cat.Name == searchString);
    }

    /// <summary>
    /// Create or update a group.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name">The group name.</param>
    /// <param name="parent">The group's parent.</param>
    /// <returns></returns>
    public static AstroObjectGroup CreateOrUpdate(AstroDbContext db, string name,
        AstroObjectGroup? parent = null)
    {
        AstroObjectGroup? group = Load(db, name);
        if (group == null)
        {
            Console.WriteLine($"Adding group {name}");
            group = new AstroObjectGroup
            {
                Name = name,
                Parent = parent
            };
            db.AstroObjectGroups.Add(group);
        }
        else
        {
            // The group was found.
            Console.WriteLine($"Updating group {name}");
            group.Parent = parent;
            db.AstroObjectGroups.Update(group);
        }
        db.SaveChanges();
        return group;
    }
}
