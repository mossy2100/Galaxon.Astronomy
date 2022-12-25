namespace Galaxon.Astronomy.Repository;

public class AtmoConstituent
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to Atmosphere (parent) object.
    public int AtmosphereId { get; set; }
    public Atmosphere Atmosphere { get; set; } = new();

    // Link to gas.
    public int MoleculeId { get; set; }
    public Molecule Molecule { get; set; } = new();

    // Percentage of gas in the atmosphere by volume.
    // Has to be nullable as we don't always know.
    public double? Percentage { get; set; }

    #endregion Properties

    /// <summary>
    /// Default constructor.
    /// </summary>
    public AtmoConstituent()
    {
    }
}
