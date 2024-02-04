using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Galaxon.Astronomy.Data.Models;

[Index(nameof(BulletinNumber), IsUnique = true)]
public class IersBulletinC
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// The IERS bulletin number.
    /// </summary>
    public int BulletinNumber { get; set; }

    /// <summary>
    /// The bulletin URL.
    /// </summary>
    [MaxLength(100)]
    public string BulletinUrl { get; set; } = "";

    /// <summary>
    /// The datetime the bulletin was downloaded and parsed.
    /// </summary>
    public DateTime DateTimeParsed { get; set; }

    /// <summary>
    /// This will be:
    ///     0 for no leap second
    ///     1 for a positive leap second (37 so far at the time of coding)
    ///    -1 for a negative leap second (none so far, but possible within 12 years)
    /// </summary>
    public sbyte Value { get; set; }

    /// <summary>
    /// The date the leap second will be inserted (or skipped), if there is one.
    /// </summary>
    [Column(TypeName = "date")]
    public DateOnly? LeapSecondDate { get; set; }
}
