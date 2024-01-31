namespace Galaxon.Astronomy.Data.Models;

public class EasterDate
{
    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateOnly Date { get; set; }
}
