namespace Galaxon.Astronomy.Data.Models;

public class SeasonalMarker
{
    public int Id { get; set; }

    /// <summary>
    /// This value is:
    ///   0 = March equinox
    ///   1 = June solstice
    ///   2 = September equinox
    ///   3 = December solstice
    /// </summary>
    [Column(TypeName = "tinyint")]
    public int MarkerNumber { get; set; }

    /// <summary>
    /// The UTC datetime of the seasonal marker.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime UtcDateTime { get; set; }
}
