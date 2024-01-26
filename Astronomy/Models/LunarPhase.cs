using Galaxon.Astronomy.Enums;

namespace Galaxon.Astronomy.Models;

public class LunarPhase
{
    #region Properties

    public int Id { get; set; }

    /// <summary>
    /// This value is:
    ///   0 = new moon
    ///   1 = first quarter
    ///   2 = full moon
    ///   3 = third quarter
    /// </summary>
    [Column(TypeName = "tinyint")]
    public ELunarPhase PhaseNumber { get; set; }

    /// <summary>
    /// The UTC datetime of the lunar phase.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime UtcDateTime { get; set; }

    #endregion Properties
}
