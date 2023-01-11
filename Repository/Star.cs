using System.Data;
using Galaxon.Core.Numbers;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;

namespace Galaxon.Astronomy.Repository;

public class Star : AstroObject
{
    /// <summary>
    /// Load a star from the database using its name.
    /// </summary>
    /// <param name="db">The DbContext.</param>
    /// <param name="name">The name of the star.</param>
    /// <returns>The Star object.</returns>
    /// <exception cref="DataException">If a matching record was not found.</exception>
    //public static Star? Load(AstroDbContext db, string searchString)
    //{
    //    return db.Stars
    //        .Include(star => star.Groups)
    //        .Include(star => star.Physical)
    //        .Include(star => star.Orbit)
    //        .Include(star => star.Rotation)
    //        .Include(star => star.Observation)
    //        .Include(star => star.Atmosphere)
    //        .Include(star => star.Stellar)
    //        .FirstOrDefault(star => star.Name == searchString);
    //}
    public static Star? Load(AstroDbContext db, string name) => AstroObject.Load(db.Stars, name);

    /// <summary>
    /// Initialize the Stars data, which for now just means adding the Sun to
    /// the database.
    /// </summary>
    public static void InitializeData()
    {
        using AstroDbContext db = new();
        Star? sun = Load(db, "Sun");
        if (sun == null)
        {
            Console.WriteLine("Adding the Sun to the database.");
            sun = new Star { Name = "Sun" };
            db.Stars.Add(sun);
        }
        else
        {
            Console.WriteLine("Updating the Sun in the database.");
        }

        // Set the Sun details.
        sun.AddToGroup(db, "Star");
        // Save the Sun object so it has an Id.
        db.SaveChanges();

        // Stellar parameters.
        sun.Stellar ??= new Stellar();
        // Spectral class.
        sun.Stellar.SpectralClass = "G2V";
        // Metallicity.
        sun.Stellar.Metallicity = 0.0122;
        // Luminosity in watts.
        sun.Stellar.Luminosity = 3.828e26;
        // Mean radiance in W/m2/sr.
        sun.Stellar.Radiance = 2.009e7;
        db.SaveChanges();

        // Observational parameters.
        sun.Observation ??= new Observation();
        // Apparent magnitude.
        sun.Observation.MinApparentMag = -26.74;
        sun.Observation.MaxApparentMag = -26.74;
        // Absolute magnitude.
        sun.Observation.AbsMag = 4.83;
        // Angular diameter.
        sun.Observation.MinAngularDiam = Angle.DegToRad(0.527);
        sun.Observation.MaxAngularDiam = Angle.DegToRad(0.545);
        db.SaveChanges();

        // Physical parameters.
        sun.Physical ??= new Physical();
        // Size and shape.
        sun.Physical.SetSphere(695_700_000);
        // Flattening.
        sun.Physical.Flattening = 9e-6;
        // Surface area in m2.
        sun.Physical.SurfaceArea = 6.09e18;
        // Volume in m3.
        sun.Physical.Volume = 1.41e27;
        // Mass in kg.
        sun.Physical.Mass = 1.9885e30;
        // Density in kg/m3.
        sun.Physical.Density = Density.GramsPerCm3ToKgPerM3(1.408);
        // Gravity in m/s2.
        sun.Physical.SurfaceGrav = 274;
        // Moment of inertia factor.
        sun.Physical.MomentOfInertiaFactor = 0.070;
        // Escape velocity in m/s.
        sun.Physical.EscapeVelocity = 617_700;
        // Standard gravitational parameter in m3/s2.
        sun.Physical.StdGravParam = 1.327_124_400_18e20;
        // Color B-V
        sun.Physical.ColorBV = 0.63;
        // Magnetic field.
        sun.Physical.HasGlobalMagField = true;
        // Ring system.
        sun.Physical.HasRingSystem = false;
        // Mean surface temperature (photosphere).
        sun.Physical.MeanSurfaceTemp = 5772;
        db.SaveChanges();

        // Orbital parameters.
        sun.Orbit ??= new Orbit();
        // 29,000 light years in metres (rounded to 2 significant figures).
        sun.Orbit.SemiMajorAxis = XDouble.RoundSigFigs(29e3 * Length.MetresPerLightYear, 2);
        // 230 million years in seconds (rounded to 2 significant figures).
        sun.Orbit.SiderealOrbitPeriod = XDouble.RoundSigFigs(230e6 * XTimeSpan.SecondsPerYear, 2);
        // Orbital speed in m/s.
        sun.Orbit.AvgOrbitSpeed = 251e3;
        db.SaveChanges();

        // Rotational parameters.
        sun.Rotation ??= new Rotation();
        // Obliquity in radians.
        sun.Rotation.Obliquity = Angle.DegToRad(7.25);
        // North pole location in radians.
        sun.Rotation.NorthPoleRightAscension = Angle.DegToRad(286.13);
        sun.Rotation.NorthPoleDeclination = Angle.DegToRad(63.87);
        // Sidereal rotation period in seconds.
        sun.Rotation.SiderealRotationPeriod = 25.05 * XTimeSpan.SecondsPerDay;
        // Equatorial rotation velocity in m/s.
        sun.Rotation.EquatRotationVelocity = 1997;
        db.SaveChanges();

        // Atmosphere.
        sun.Atmosphere ??= new Atmosphere();
        // Make sure all the molecules are in the database first.
        Molecule.CreateOrUpdate(db, "hydrogen", "H");
        Molecule.CreateOrUpdate(db, "helium", "He");
        Molecule.CreateOrUpdate(db, "oxygen", "O");
        Molecule.CreateOrUpdate(db, "carbon", "C");
        Molecule.CreateOrUpdate(db, "iron", "Fe");
        Molecule.CreateOrUpdate(db, "neon", "Ne");
        Molecule.CreateOrUpdate(db, "nitrogen", "N");
        Molecule.CreateOrUpdate(db, "silicon", "Si");
        Molecule.CreateOrUpdate(db, "magnesium", "Mg");
        Molecule.CreateOrUpdate(db, "sulphur", "S");
        // Add the constituents.
        sun.Atmosphere.AddConstituent(db, "H", 73.46);
        sun.Atmosphere.AddConstituent(db, "He", 24.85);
        sun.Atmosphere.AddConstituent(db, "O", 0.77);
        sun.Atmosphere.AddConstituent(db, "C", 0.29);
        sun.Atmosphere.AddConstituent(db, "Fe", 0.16);
        sun.Atmosphere.AddConstituent(db, "Ne", 0.12);
        sun.Atmosphere.AddConstituent(db, "N", 0.09);
        sun.Atmosphere.AddConstituent(db, "Si", 0.07);
        sun.Atmosphere.AddConstituent(db, "Mg", 0.05);
        sun.Atmosphere.AddConstituent(db, "S", 0.04);
        sun.Atmosphere.IsSurfaceBoundedExosphere = false;
        sun.Atmosphere.ScaleHeight = 140e3;
        db.SaveChanges();
    }
}
