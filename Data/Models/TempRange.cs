namespace Galaxon.Astronomy.Data.Models;

/// <summary>
/// Represents temperature range information.
/// </summary>
public class TempRange
{
    /// <summary>
    /// Gets or sets the primary key of the temperature range.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the minimum temperature in the range.
    /// </summary>
    public double? Min { get; set; }

    /// <summary>
    /// Gets or sets the mean temperature in the range.
    /// </summary>
    public double? Mean { get; set; }

    /// <summary>
    /// Gets or sets the maximum temperature in the range.
    /// </summary>
    public double? Max { get; set; }
}
