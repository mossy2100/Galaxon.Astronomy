namespace Galaxon.Astronomy.Data.Models;

public class LeapSecond
{
    public int Id { get; set; }

    [Column(TypeName = "date")]
    public DateOnly IersBulletinCDate { get; set; }

    /// <summary>
    /// This will be:
    ///    -1 for a negative leap second (none so far, but possible within 12 years)
    ///     0 for no leap second
    ///     1 for a positive leap second (37 so far at the time of coding)
    /// </summary>
    public sbyte Value { get; set; }

    /// <summary>
    /// The date the leap second will be inserted, if there is one.
    /// </summary>
    [Column(TypeName = "date")]
    public DateOnly? LeapSecondDate { get; set; }
}
