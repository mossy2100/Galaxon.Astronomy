using Galaxon.Core.Exceptions;
using Galaxon.Core.Strings;
using Microsoft.EntityFrameworkCore;

namespace Galaxon.Astronomy.Repository;

// Main class for astronomical objects.
// All physical quantities are in SI units.
public abstract class AstroObject
{
    public AstroObject()
    {
        Type = GetType().Name;
    }

    /// <summary>
    /// Specify the object's orbital parameters.
    /// </summary>
    /// <param name="parent">The object it orbits.</param>
    /// <param name="Q">The periapsis in km.</param>
    /// <param name="q">The apoapsis in km.</param>
    /// <param name="a">The semi-major axis in km.</param>
    /// <param name="e">The orbital eccentricity.</param>
    public void SetOrbit(AstroObject parent, ulong? Q, ulong? q, ulong? a, double? e)
    {
        // Guard clauses.
        if (Q != null && q != null)
        {
            if (Q < q)
            {
                throw new ArgumentException(
                    "The apoapsis should be greater than or equal to the periapsis.");
            }
            if (a != null && (a < q || a > Q))
            {
                throw new ArgumentException(
                    "The semi-major axis should be greater than or equal to the periapsis and less than or equal to the apoapsis.");
            }
        }

        Parent = parent;

        Orbit ??= new Orbit();
        Orbit.Apoapsis = Q;
        Orbit.Periapsis = q;
        Orbit.SemiMajorAxis = a;
        Orbit.Eccentricity = e;
    }

    /// <summary>
    /// Check if the object is in a certain group.
    /// </summary>
    /// <param name="group">The group to check.</param>
    /// <returns>If the object is in the specified group.</returns>
    public bool IsInGroup(AstroObjectGroup group)
    {
        return Groups?.Contains(group) ?? false;
    }

    /// <summary>
    /// Check if the object is in a certain group.
    /// </summary>
    /// <param name="groupName">The name of the group to check (case
    /// sensitive).</param>
    /// <returns>If the object is in the specified group.</returns>
    public bool IsInGroup(string groupName)
    {
        return Groups != null && Groups
            .Count(cat => groupName.EqualsIgnoreCase(cat.Name)) > 0;
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    /// <param name="group">The group.</param>
    public void AddToGroup(AstroObjectGroup group)
    {
        if (!IsInGroup(group))
        {
            Groups ??= new List<AstroObjectGroup>();
            Groups.Add(group);
        }
    }

    /// <summary>
    /// Add the object to a group, if it's not already a member.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="groupName">The group name.</param>
    public void AddToGroup(AstroDbContext db, string groupName)
    {
        var group = AstroObjectGroup.Load(db, groupName);
        if (group == null)
        {
            throw new DataNotFoundException($"Group '{groupName}' not found.");
        }
        AddToGroup(group);
    }

    /// <summary>
    /// Convert the object to a string.
    /// </summary>
    /// <returns>The object name, or the readable designation for
    /// planetoids.</returns>
    public override string ToString()
    {
        if (this is Planetoid)
        {
            string? readable = ((Planetoid)this).MinorPlanetRecord?.ReadableDesignation;
            if (!string.IsNullOrEmpty(readable))
            {
                return readable;
            }
        }
        return Name ?? "Unknown";
    }

    /// <summary>
    /// Load an AstroObject from the database using a search function.
    /// </summary>
    /// <param name="set">The DbSet to search.</param>
    /// <param name="searchFunc">The search function.</param>
    /// <exception cref="ArgumentNullException">If the search string == null or
    /// empty.</exception>
    public static T? Load<T>(IEnumerable<T> set, Func<AstroObject, bool> searchFunc)
        where T : AstroObject
    {
        return (T?)set.FirstOrDefault(searchFunc);
    }

    /// <summary>
    /// Load an AstroObject from the database using a function.
    /// </summary>
    /// <param name="set">The DbSet to search.</param>
    /// <param name="name">The object's name (case-insensitive).</param>
    /// <returns>The matching AstroObject.</returns>
    /// <exception cref="ArgumentNullException">If the search string == null or
    /// empty.</exception>
    public static T? Load<T>(DbSet<T> set, string name) where T : AstroObject
    {
        // Guard.
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name), "Name cannot be empty.");
        }

        // Get the search function.
        Func<AstroObject, bool> searchFunc =
            ao => (ao.Name ?? "").ToLower() == name.ToLower();

        return Load(set, searchFunc);
    }

    /// <summary>
    /// Load an AstroObject from the database using its number.
    /// </summary>
    /// <param name="set">The DbSet to search.</param>
    /// <param name="number">The object's number.</param>
    /// <returns>The matching AstroObject.</returns>
    /// <exception cref="ArgumentNullException">If the search number is negative
    /// or zero.</exception>
    public static T? Load<T>(DbSet<T> set, uint number) where T : AstroObject
    {
        // Guard.
        if (number == 0)
        {
            throw new ArgumentNullException(nameof(number), "Number cannot be zero.");
        }

        // Get the search function.
        Func<AstroObject, bool> searchFunc = ao => (ao.Number ?? 0) == number;

        return Load(set, searchFunc);
    }

    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Object name, e.g. "Sun", "Earth", "Ceres".
    // These are not assumed to be unique.
    [Column(TypeName = "varchar(50)")]
    public string? Name { get; set; }

    // Object number, e.g. 2 (Venus), 301 (Luna), 1 (Ceres).
    // These numbers aren't unique, and not every object will have one.
    [Column(TypeName = "int")]
    public uint? Number { get; set; }

    // The object type, which is also the class name.
    [Column(TypeName = "varchar(20)")]
    public string Type { get; }

    // The groups (populations/categories) this object belongs to.
    public List<AstroObjectGroup>? Groups { get; set; }

    // Link to physical parameters object.
    public Physical? Physical { get; set; }

    // Link to rotational parameters object.
    public Rotation? Rotation { get; set; }

    // Link to orbital parameters object.
    public Orbit? Orbit { get; set; }

    // Link to parent object (i.e. the object being orbited).
    public int? ParentId { get; set; }

    public AstroObject? Parent { get; set; }

    // Link to child objects (i.e. the objects orbiting this object).
    public List<AstroObject>? Children { get; set; }

    // Link to observational parameters object.
    public Observation? Observation { get; set; }

    // Link to atmosphere object.
    public Atmosphere? Atmosphere { get; set; }

    // Link to stellar parameters object.
    public Stellar? Stellar { get; set; }

    // Link to Minor Planet Center record.
    public MinorPlanetRecord? MinorPlanetRecord { get; set; }

    // VSOP87D records.
    public List<VSOP87DRecord>? VSOP87DRecords { get; set; }

    #endregion Properties
}
