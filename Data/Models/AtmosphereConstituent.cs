namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// Represents a constituent of the atmosphere.
/// </summary>
public class AtmosphereConstituent
{
    /// <summary>
    /// Gets or sets the primary key of the atmosphere constituent.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the foreign key referencing the parent AtmosphereRecord object.
    /// </summary>
    public int AtmosphereId { get; set; }

    /// <summary>
    /// Gets or sets the reference to the parent AtmosphereRecord object.
    /// </summary>
    public AtmosphereRecord Atmosphere { get; set; } = new AtmosphereRecord();

    /// <summary>
    /// Gets or sets the link to the gas molecule.
    /// </summary>
    public int MoleculeId { get; set; }

    /// <summary>
    /// Gets or sets the reference to the gas molecule.
    /// </summary>
    public Molecule Molecule { get; set; } = new Molecule();

    /// <summary>
    /// Gets or sets the percentage of the gas in the atmosphere by volume.
    /// </summary>
    /// <remarks>
    /// This property is nullable as the exact percentage is not always known.
    /// </remarks>
    public double? Percentage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtmosphereConstituent"/> class.
    /// </summary>
    public AtmosphereConstituent() { }
}
