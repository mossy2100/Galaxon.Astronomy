namespace Galaxon.Astronomy.Models;

public class EasterDate
{
    #region Properties

    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateOnly Date { get; set; }

    #endregion Properties
}
