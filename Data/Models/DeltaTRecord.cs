namespace Galaxon.Astronomy.Models;

public class DeltaTRecord
{
    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "smallint")]
    public int Year { get; set; }

    public double DeltaT { get; set; }

    #endregion Properties
}
