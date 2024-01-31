namespace Galaxon.Astronomy.Data.Models;

public class DeltaTRecord
{
    public int Id { get; set; }

    [Column(TypeName = "smallint")]
    public int Year { get; set; }

    public double DeltaT { get; set; }
}
