using Galaxon.Core.Strings;

namespace Galaxon.Astronomy.Models;

// Main class for astronomical objects.
// All physical quantities are in SI units.
public class AstroObject
{
    public AstroObject(string? name = null, uint? number = null)
    {
        Name = name;
        Number = number;
    }

    // ---------------------------------------------------------------------------------------------
    // Core details

    // Primary key.
    public int Id { get; set; }

    // Object name, e.g. "Sun", "Earth", "Ceres".
    // These are not assumed to be unique, and in some cases (minor planets) can be null.
    [Column(TypeName = "varchar(50)")]
    public string? Name { get; set; }

    // Object number, e.g. 2 (Venus), 301 (Luna), 1 (Ceres).
    // These numbers also aren't unique, and in some cases (stars, major planets) can be null.
    [Column(TypeName = "int")]
    public uint? Number { get; set; }

    // ---------------------------------------------------------------------------------------------
    // Relationships

    // Link to parent object (i.e. the object being orbited).
    public int? ParentId { get; set; }

    public AstroObject? Parent { get; set; }

    // Link to child objects (i.e. the objects orbiting this object).
    public List<AstroObject>? Children { get; set; }

    // The groups (populations/categories) this object belongs to.
    public List<AstroObjectGroup>? Groups { get; set; }

    // ---------------------------------------------------------------------------------------------
    // Additional properties records

    // Link to physical properties record.
    public PhysicalRecord? Physical { get; set; }

    // Link to rotational properties record.
    public RotationalRecord? Rotation { get; set; }

    // Link to orbital properties record.
    public OrbitalRecord? Orbit { get; set; }

    // Link to observational properties record.
    public ObservationalRecord? Observation { get; set; }

    // Link to atmosphere properties object.
    public AtmosphereRecord? Atmosphere { get; set; }

    // Link to stellar properties record.
    public StellarRecord? Stellar { get; set; }

    // Link to Minor Planet Center record.
    public MinorPlanetRecord? MinorPlanet { get; set; }

    // Link to matching VSOP87D records.
    public List<VSOP87DRecord>? VSOP87DRecords { get; set; }

    /// <summary>
    /// Check for case-sensitive match on name, number, packed designation, or readable
    /// designation.
    /// Only matches on full string (although with whitespace trimmed).
    /// </summary>
    /// <param name="searchString">The search string.</param>
    /// <returns>If there's a match.</returns>
    /// TODO Test.
    public bool IsMatch(string searchString)
    {
        // Guard.
        searchString = searchString.Trim();
        if (string.IsNullOrEmpty(searchString))
        {
            throw new ArgumentOutOfRangeException(nameof(searchString),
                "Cannot be null, empty, or whitespace.");
        }

        // Match on name, number, both together, readable designation, or packed designation.
        return searchString == Name
            || searchString == Number.ToString()
            || searchString == $"{Number} {Name}"
            || searchString == (MinorPlanet?.ReadableDesignation ?? "")
            || searchString == (MinorPlanet?.PackedDesignation ?? "");
    }

    /// <summary>
    /// Specify the object's orbital parameters.
    /// </summary>
    /// <param name="parent">The object it orbits.</param>
    /// <param name="periapsis">The periapsis in km.</param>
    /// <param name="apoapsis">The apoapsis in km.</param>
    /// <param name="semiMajorAxis">The semi-major axis in km.</param>
    /// <param name="eccentricity">The orbital eccentricity.</param>
    public void SetOrbit(AstroObject parent, ulong? periapsis, ulong? apoapsis,
        ulong? semiMajorAxis, double? eccentricity)
    {
        // Guard clauses.
        if (periapsis != null && apoapsis != null)
        {
            if (periapsis > apoapsis)
            {
                throw new ArgumentException(
                    "The apoapsis should be greater than or equal to the periapsis.");
            }
            if (semiMajorAxis != null && (semiMajorAxis < periapsis || semiMajorAxis > apoapsis))
            {
                throw new ArgumentException(
                    "The semi-major axis should be greater than or equal to the periapsis and less than or equal to the apoapsis.");
            }
        }

        Parent = parent;
        Orbit ??= new OrbitalRecord();
        Orbit.Periapsis = periapsis;
        Orbit.Apoapsis = apoapsis;
        Orbit.SemiMajorAxis = semiMajorAxis;
        Orbit.Eccentricity = eccentricity;
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
    /// <param name="groupName">The name of the group to check (case sensitive).</param>
    /// <returns>If the object is in the specified group.</returns>
    public bool IsInGroup(string groupName)
    {
        return Groups != null && Groups.Any(group => groupName.EqualsIgnoreCase(group.Name));
    }

    /// <summary>
    /// Convert the object to a string.
    /// </summary>
    /// <returns>The object name, or the readable designation for minor planets.</returns>
    public override string ToString()
    {
        string? readable = MinorPlanet?.ReadableDesignation;
        if (!string.IsNullOrEmpty(readable))
        {
            return readable;
        }

        if (Name != null && Number == null)
        {
            return Name;
        }

        return $"{Name} ({Number})".Trim();
    }
}
