namespace AstroMultimedia.Astronomy.Repository;

public class TempRange
{
    #region Mapped Properties

    // Primary key.
    public int Id { get; set; }

    // The minimum temperature.
    public double? Min { get; set; }

    // The mean temperature.
    public double? Mean { get; set; }

    // The maximum temperature.
    public double? Max { get; set; }

    #endregion Mapped Properties

    public TempRange()
    {
    }
}
