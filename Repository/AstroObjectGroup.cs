namespace Galaxon.Astronomy.Repository;

public class AstroObjectGroup
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Group name.
    public string Name { get; set; } = "";

    // Objects in the group (navigation property).
    public List<AstroObject> Objects { get; set; } = new();

    // Parent group.
    public int? ParentId { get; set; }
    public AstroObjectGroup? Parent { get; set; }

    #endregion Properties

    public AstroObjectGroup()
    {
    }

    /// <summary>
    /// Load an AstroObjectGroup from the database.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="searchString">The group name.</param>
    /// <returns>The found object or null.</returns>
    public static AstroObjectGroup? Load(AstroDbContext db, string searchString) =>
        db.AstroObjectGroups.FirstOrDefault(cat => cat.Name == searchString);

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

    /// <summary>
    /// Initialize all the groups.
    /// </summary>
    public static void InitializeData()
    {
        using AstroDbContext db = new();

        // Stars.
        AstroObjectGroup star = CreateOrUpdate(db, "Star");
        CreateOrUpdate(db, "Hypergiant", star);
        CreateOrUpdate(db, "Supergiant", star);

        AstroObjectGroup giantStar = CreateOrUpdate(db, "Giant", star);
        CreateOrUpdate(db, "Subgiant", giantStar);
        CreateOrUpdate(db, "Bright giant", giantStar);
        CreateOrUpdate(db, "Red giant", giantStar);
        CreateOrUpdate(db, "Yellow giant", giantStar);
        CreateOrUpdate(db, "Blue giant", giantStar);
        CreateOrUpdate(db, "White giant", giantStar);

        AstroObjectGroup mainSequence = CreateOrUpdate(db, "Main sequence", star);
        CreateOrUpdate(db, "Red dwarf", mainSequence);
        CreateOrUpdate(db, "Orange dwarf", mainSequence);
        CreateOrUpdate(db, "Yellow dwarf", mainSequence);
        CreateOrUpdate(db, "Blue main sequence star", mainSequence);
        CreateOrUpdate(db, "White dwarf", mainSequence);

        CreateOrUpdate(db, "Subdwarf", star);
        CreateOrUpdate(db, "Brown dwarf", star);

        // Planets.
        AstroObjectGroup planet = CreateOrUpdate(db, "Planet");
        CreateOrUpdate(db, "Terrestrial planet", planet);

        AstroObjectGroup giantPlanet = CreateOrUpdate(db, "Giant planet", planet);
        CreateOrUpdate(db, "Gas giant", giantPlanet);
        CreateOrUpdate(db, "Ice giant", giantPlanet);

        // Planetoids.
        AstroObjectGroup minorPlanet = CreateOrUpdate(db, "Minor planet");
        CreateOrUpdate(db, "Centaur", minorPlanet);
        CreateOrUpdate(db, "Trojan", minorPlanet);
        CreateOrUpdate(db, "Quasi-satellite", minorPlanet);

        AstroObjectGroup dwarfPlanet = CreateOrUpdate(db, "Dwarf planet", minorPlanet);
        CreateOrUpdate(db, "Plutoid", dwarfPlanet);

        AstroObjectGroup asteroid = CreateOrUpdate(db, "Asteroid", minorPlanet);
        CreateOrUpdate(db, "Potentially hazardous asteroid", asteroid);

        AstroObjectGroup nea = CreateOrUpdate(db, "Near Earth asteroid", asteroid);
        CreateOrUpdate(db, "Apohele asteroid", nea);
        CreateOrUpdate(db, "Aten asteroid", nea);
        CreateOrUpdate(db, "Apollo asteroid", nea);
        CreateOrUpdate(db, "Amor asteroid", nea);

        AstroObjectGroup sssb = CreateOrUpdate(db, "Small Solar System body");
        CreateOrUpdate(db, "Comet", sssb);

        AstroObjectGroup tno = CreateOrUpdate(db, "Trans-Neptunian Object");
        CreateOrUpdate(db, "Oort cloud", tno);

        AstroObjectGroup kbo = CreateOrUpdate(db, "Kuper Belt Object", tno);
        CreateOrUpdate(db, "Cubewano", kbo);
        AstroObjectGroup resonentKbo = CreateOrUpdate(db, "Resonant KBO", kbo);
        CreateOrUpdate(db, "Plutino", resonentKbo);

        AstroObjectGroup sdo = CreateOrUpdate(db, "Scattered-disc object", tno);
        CreateOrUpdate(db, "Resonant SDO", sdo);

        AstroObjectGroup etno = CreateOrUpdate(db, "Extreme Trans-Neptunian object", tno);
        AstroObjectGroup detached = CreateOrUpdate(db, "Detached object", etno);
        CreateOrUpdate(db, "Sednoid", detached);

        // Satellites.
        AstroObjectGroup satellite = CreateOrUpdate(db, "Satellite");
        CreateOrUpdate(db, "Regular satellite", satellite);
        CreateOrUpdate(db, "Irregular satellite", satellite);
        CreateOrUpdate(db, "Prograde satellite", satellite);
        CreateOrUpdate(db, "Retrograde satellite", satellite);
    }
}
