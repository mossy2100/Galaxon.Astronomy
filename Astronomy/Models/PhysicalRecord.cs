using Galaxon.Core.Numbers;

namespace Galaxon.Astronomy.Models;

public class PhysicalRecord
{
    /// <summary>
    /// Gets or sets the primary key of the physical record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the link to the astronomical object associated with this record.
    /// </summary>
    public int AstroObjectId { get; set; }

    /// <summary>
    /// Gets or sets the reference to the astronomical object associated with this record.
    /// </summary>
    public AstroObject? AstroObject { get; set; }

    /// <summary>
    /// Gets or sets the first radius in kilometers.
    /// </summary>
    public double? RadiusA { get; set; }

    /// <summary>
    /// Gets or sets the second radius in kilometers.
    /// </summary>
    public double? RadiusB { get; set; }

    /// <summary>
    /// Gets or sets the third radius in kilometers.
    /// </summary>
    public double? RadiusC { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the object is gravitationally rounded (in hydrostatic
    /// equilibrium).
    /// </summary>
    public bool? IsRound { get; set; }

    /// <summary>
    /// Gets or sets the mean radius in kilometers.
    /// </summary>
    public double? MeanRadius { get; set; }

    /// <summary>
    /// Gets the equatorial radius. Only valid for spheroidal objects.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the object is not a spheroid.</exception>
    [NotMapped]
    public double EquatorialRadius
    {
        get
        {
            if (IsRound == null || RadiusA == null || RadiusB == null ||
                RadiusC == null)
            {
                throw new InvalidOperationException("Specify the size and shape first.");
            }

            // Check it's round.
            if (IsRound.Value)
            {
                // Find two radii the same.
                if (RadiusA.FuzzyEquals(RadiusB) || RadiusA.FuzzyEquals(RadiusC))
                {
                    return RadiusA.Value;
                }
                if (RadiusB.FuzzyEquals(RadiusC))
                {
                    return RadiusB.Value;
                }
            }

            // Object is not a spheroid.
            throw new InvalidOperationException(
                "Can only get the equatorial radius for a spherical or spheroidal object.");
        }
    }

    /// <summary>
    /// Gets the polar radius. Only valid for spheroidal objects.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the object is not a spheroid.</exception>
    [NotMapped]
    public double PolarRadius
    {
        get
        {
            if (IsRound == null || RadiusA == null || RadiusB == null || RadiusC == null)
            {
                throw new InvalidOperationException("Specify the size and shape first.");
            }

            // Check it's round.
            if (IsRound.Value)
            {
                // Which one is different?
                if (RadiusA.FuzzyEquals(RadiusB))
                {
                    return RadiusC.Value;
                }
                if (RadiusA.FuzzyEquals(RadiusC))
                {
                    return RadiusB.Value;
                }
                if (RadiusB.FuzzyEquals(RadiusC))
                {
                    return RadiusA.Value;
                }
            }

            // Object is not a spheroid.
            throw new InvalidOperationException(
                "Can only get the polar radius for a spherical or spheroidal object.");
        }
    }

    // Flattening.
    public double? Flattening { get; set; }

    // Surface area in km^2.
    public double? SurfaceArea { get; set; }

    // Volume in km^3.
    public double? Volume { get; set; }

    // Mass in kg.
    public double? Mass { get; set; }

    // Mean density in g/cm^3.
    public double? Density { get; set; }

    // Surface gravity in m/s2.
    public double? SurfaceGrav { get; set; }

    // Escape velocity in km/s.
    public double? EscapeVelocity { get; set; }

    // Standard gravitational parameter in m3/s2.
    public double? StdGravParam { get; set; }

    // Moment of inertia factor.
    public double? MomentOfInertiaFactor { get; set; }

    // Has global magnetic field?
    public bool? HasGlobalMagField { get; set; }

    // Has ring system?
    public bool? HasRingSystem { get; set; }

    // Solar irradiance (W/m2).
    public double? SolarIrradiance { get; set; }

    // Geometric albedo.
    public double? GeometricAlbedo { get; set; }

    // Color (B-V).
    public double? ColorBV { get; set; }

    // Color (U-B).
    public double? ColorUB { get; set; }

    // Minimum surface temperature (K) (0.1 bar for giant planets).
    public double? MinSurfaceTemp { get; set; }

    // Mean surface temperature (K) (0.1 bar for giant planets).
    public double? MeanSurfaceTemp { get; set; }

    // Maximum surface temperature (K) (0.1 bar for giant planets).
    public double? MaxSurfaceTemp { get; set; }

    /// <summary>
    /// Specify the object's size and shape.
    /// This can be used for any object. The other methods are for convenience.
    /// <see cref="SetSphericalShape"/>
    /// <see cref="SetSpheroidalShape"/>
    /// <see cref="SetEllipsoidalShape"/>
    /// <see cref="SetNonEllipsoidShape"/>
    /// </summary>
    /// <param name="radiusA">The first radius (or half length).</param>
    /// <param name="radiusB">The second radius (or half width).</param>
    /// <param name="radiusC">The third radius (or half height).</param>
    /// <param name="isRound">
    /// This flag should be:
    ///   - true for round objects (stars, planets, dwarf planets, satellite planets)
    ///   - false for lumpy objects (small bodies, satellite planetoids)
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">If any of the radii are 0 or negative.</exception>
    public void SetSizeAndShape(double radiusA, double radiusB, double radiusC, bool isRound)
    {
        if (radiusA <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radiusA), "Must be a positive value.");
        }
        if (radiusB <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radiusB), "Must be a positive value.");
        }
        if (radiusC <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(radiusC), "Must be a positive value.");
        }

        RadiusA = radiusA;
        RadiusB = radiusB;
        RadiusC = radiusC;
        IsRound = isRound;
    }

    /// <summary>
    /// Specify the object is a sphere.
    /// </summary>
    /// <param name="radius">The radius.</param>
    public void SetSphericalShape(double radius)
    {
        SetEllipsoidalShape(radius, radius, radius);
    }

    /// <summary>
    /// Specify the object is a spheroid.
    /// </summary>
    /// <param name="radiusEquat">The equatorial radius in km.</param>
    /// <param name="radiusPolar">The polar radius in km.</param>
    public void SetSpheroidalShape(double radiusEquat, double radiusPolar)
    {
        SetEllipsoidalShape(radiusEquat, radiusEquat, radiusPolar);
    }

    /// <summary>
    /// Specify the object as a scalene ellipsoid.
    /// </summary>
    /// <param name="radiusA">The first radius in km.</param>
    /// <param name="radiusB">The second radius in km.</param>
    /// <param name="radiusC">The third radius in km.</param>
    public void SetEllipsoidalShape(double radiusA, double radiusB, double radiusC)
    {
        SetSizeAndShape(radiusA, radiusB, radiusC, true);

        // Calculate some stuff; they can still set the property directly if
        // they want to override this.
        var ellipsoid = new Ellipsoid(radiusA, radiusB, radiusC);

        // Volumetric mean radius.
        MeanRadius = ellipsoid.VolumetricMeanRadius;

        // Surface area.
        SurfaceArea = ellipsoid.SurfaceArea;

        // Volume.
        Volume = ellipsoid.Volume;
    }

    /// <summary>
    /// Specify the object's shape as irregular (not round).
    /// </summary>
    /// <param name="length">The length in km.</param>
    /// <param name="width">The width in km.</param>
    /// <param name="height">The height in km.</param>
    public void SetNonEllipsoidShape(double length, double width, double height)
    {
        SetSizeAndShape(length / 2, width / 2, height / 2, false);
    }
}
