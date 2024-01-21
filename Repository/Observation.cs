namespace Galaxon.Astronomy.Repository;

public class Observation
{
    #region Properties

    // Primary key.
    public int Id { get; set; }

    // Link to owner.
    public int AstroObjectId { get; set; }

    public AstroObject? AstroObject { get; set; }

    // Absolute magnitude.
    public double? AbsMag { get; set; }

    // Minimum apparent magnitude.
    public double? MinApparentMag { get; set; }

    // Maximum apparent magnitude.
    public double? MaxApparentMag { get; set; }

    // Minimum angular diameter (°).
    public double? MinAngularDiam { get; set; }

    // Maximum angular diameter (°).
    public double? MaxAngularDiam { get; set; }

    #endregion Properties
}
