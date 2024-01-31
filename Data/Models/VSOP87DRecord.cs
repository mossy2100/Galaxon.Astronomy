namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// Represents a record containing VSOP87D planetary data.
/// </summary>
public class VSOP87DRecord
{
    /// <summary>
    /// Gets or sets the primary key of the VSOP87D record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the planet associated with this record.
    /// </summary>
    [Column(TypeName = "varchar(10)")]
    public string PlanetName { get; set; } = "";

    /// <summary>
    /// Gets or sets the link to the astronomical object associated with this record.
    /// </summary>
    public int AstroObjectId { get; set; }

    /// <summary>
    /// Gets or sets the reference to the astronomical object associated with this record.
    /// </summary>
    public AstroObject? AstroObject { get; set; }

    /// <summary>
    /// Gets or sets the variable used in the record.
    /// </summary>
    [Column(TypeName = "char(1)")]
    public char Variable { get; set; }

    /// <summary>
    /// Gets or sets the exponent used in the record.
    /// </summary>
    [Column(TypeName = "tinyint")]
    public byte Exponent { get; set; }

    /// <summary>
    /// Gets or sets the index used in the record.
    /// </summary>
    [Column(TypeName = "smallint")]
    public ushort Index { get; set; }

    /// <summary>
    /// Gets or sets the amplitude value in the record.
    /// </summary>
    public double Amplitude { get; set; }

    /// <summary>
    /// Gets or sets the phase value in the record.
    /// </summary>
    public double Phase { get; set; }

    /// <summary>
    /// Gets or sets the frequency value in the record.
    /// </summary>
    public double Frequency { get; set; }
}
