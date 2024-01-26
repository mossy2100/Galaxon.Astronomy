namespace Galaxon.Astronomy.Models;

public class MinorPlanetRecord
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }

    public AstroObject? AstroObject { get; set; }

    // The designation in packed form.
    public string PackedDesignation { get; set; } = "";

    // The designation in readable form.
    public string ReadableDesignation { get; set; } = "";

    // Date/time of periapsis (minor planets).
    public DateTime? DatePeriapsis { get; set; }

    // Orbit type (minor planets).
    public byte? OrbitType { get; set; }

    // Is 1-opposition object seen at earlier opposition
    public bool? Is1Opp { get; set; }

    // Is it a critical-list numbered object?
    public bool? IsCriticalListNumberedObject { get; set; }

    // The object a trojan or quasi-satellite is co-orbital with.
    public int? CoOrbitalObjectId { get; set; }

    public AstroObject? CoOrbitalObject { get; set; }

    // Tholen spectral type.
    public string? Tholen { get; set; }

    // SMASS spectral type.
    public string? SMASS { get; set; }

    #endregion Properties
}
