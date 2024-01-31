using Galaxon.Astronomy.Data;
using Galaxon.Astronomy.Data.Repositories;
using Galaxon.Astronomy.Data.Models;
using Galaxon.Core.Numbers;
using Galaxon.Core.Time;
using Galaxon.Numerics.Geometry;
using Galaxon.Quantities;

namespace Galaxon.Astronomy.Algorithms;

public class SunService(
    AstroDbContext astroDbContext,
    AstroObjectRepository astroObjectRepository,
    AstroObjectGroupRepository astroObjectGroupRepository,
    EarthService earthService,
    PlanetService planetService)
{
    /// <summary>
    /// Initialize the Stars data, which for now just means adding the Sun to the database.
    /// </summary>
    public void InitializeData()
    {
        AstroObject? sun = astroObjectRepository.Load("Sun");
        if (sun == null)
        {
            Console.WriteLine("Adding the Sun to the database.");
            // Create tne new object.
            sun = new AstroObject("Sun");
        }
        else
        {
            Console.WriteLine("Updating the Sun in the database.");
        }

        // Set the Sun's basic details.
        astroObjectGroupRepository.AddToGroup(sun, "star");

        // Save the Sun object so it has an Id.
        astroDbContext.SaveChanges();

        // Stellar parameters.
        sun.Stellar ??= new StellarRecord();
        // Spectral class.
        sun.Stellar.SpectralClass = "G2V";
        // Metallicity.
        sun.Stellar.Metallicity = 0.0122;
        // Luminosity in watts.
        sun.Stellar.Luminosity = 3.828e26;
        // Mean radiance in W/m2/sr.
        sun.Stellar.Radiance = 2.009e7;
        astroDbContext.SaveChanges();

        // Observational parameters.
        sun.Observation ??= new ObservationalRecord();
        // Apparent magnitude.
        sun.Observation.MinApparentMag = -26.74;
        sun.Observation.MaxApparentMag = -26.74;
        // Absolute magnitude.
        sun.Observation.AbsMag = 4.83;
        // Angular diameter.
        sun.Observation.MinAngularDiam = Angle.DegToRad(0.527);
        sun.Observation.MaxAngularDiam = Angle.DegToRad(0.545);
        astroDbContext.SaveChanges();

        // Physical parameters.
        sun.Physical ??= new PhysicalRecord();
        // Size and shape.
        sun.Physical.SetSphericalShape(695_700_000);
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
        astroDbContext.SaveChanges();

        // Orbital parameters.
        sun.Orbit ??= new OrbitalRecord();
        // 29,000 light years in metres (rounded to 2 significant figures).
        sun.Orbit.SemiMajorAxis = XDouble.RoundSigFigs(29e3 * Length.MetresPerLightYear, 2);
        // 230 million years in seconds (rounded to 2 significant figures).
        sun.Orbit.SiderealOrbitPeriod = XDouble.RoundSigFigs(230e6 * XTimeSpan.SECONDS_PER_YEAR, 2);
        // Orbital speed in m/s.
        sun.Orbit.AvgOrbitSpeed = 251e3;
        astroDbContext.SaveChanges();

        // Rotational parameters.
        sun.Rotation ??= new RotationalRecord();
        // Obliquity in radians.
        sun.Rotation.Obliquity = Angle.DegToRad(7.25);
        // North pole location in radians.
        sun.Rotation.NorthPoleRightAscension = Angle.DegToRad(286.13);
        sun.Rotation.NorthPoleDeclination = Angle.DegToRad(63.87);
        // Sidereal rotation period in seconds.
        sun.Rotation.SiderealRotationPeriod = 25.05 * XTimeSpan.SECONDS_PER_DAY;
        // Equatorial rotation velocity in m/s.
        sun.Rotation.EquatRotationVelocity = 1997;
        astroDbContext.SaveChanges();

        // Atmosphere.
        sun.Atmosphere ??= new AtmosphereRecord();
        // Make sure all the molecules are in the database first.
        Molecule.CreateOrUpdate(astroDbContext, "hydrogen", "H");
        Molecule.CreateOrUpdate(astroDbContext, "helium", "He");
        Molecule.CreateOrUpdate(astroDbContext, "oxygen", "O");
        Molecule.CreateOrUpdate(astroDbContext, "carbon", "C");
        Molecule.CreateOrUpdate(astroDbContext, "iron", "Fe");
        Molecule.CreateOrUpdate(astroDbContext, "neon", "Ne");
        Molecule.CreateOrUpdate(astroDbContext, "nitrogen", "N");
        Molecule.CreateOrUpdate(astroDbContext, "silicon", "Si");
        Molecule.CreateOrUpdate(astroDbContext, "magnesium", "Mg");
        Molecule.CreateOrUpdate(astroDbContext, "sulphur", "S");
        // Add the constituents.
        sun.Atmosphere.AddConstituent(astroDbContext, "H", 73.46);
        sun.Atmosphere.AddConstituent(astroDbContext, "He", 24.85);
        sun.Atmosphere.AddConstituent(astroDbContext, "O", 0.77);
        sun.Atmosphere.AddConstituent(astroDbContext, "C", 0.29);
        sun.Atmosphere.AddConstituent(astroDbContext, "Fe", 0.16);
        sun.Atmosphere.AddConstituent(astroDbContext, "Ne", 0.12);
        sun.Atmosphere.AddConstituent(astroDbContext, "N", 0.09);
        sun.Atmosphere.AddConstituent(astroDbContext, "Si", 0.07);
        sun.Atmosphere.AddConstituent(astroDbContext, "Mg", 0.05);
        sun.Atmosphere.AddConstituent(astroDbContext, "S", 0.04);
        sun.Atmosphere.IsSurfaceBoundedExosphere = false;
        sun.Atmosphere.ScaleHeight = 140e3;
        astroDbContext.SaveChanges();
    }

    /// <summary>
    /// Calculate apparent solar latitude and longitude for a given instant
    /// specified as a Julian Date in Terrestrial Time (this is also known as a
    /// Julian Ephemeris Day or JED (or JDE), and differs from the Julian Date by ∆T).
    /// This method uses the higher accuracy algorithm from AA2 Ch25 p166
    /// (p174 in PDF)
    /// </summary>
    /// <param name="jdtt">The Julian Ephemeris Day.</param>
    /// <returns>The longitude of the Sun (Ls) in radians at the given
    /// instant.</returns>
    public (double Lng, double Lat) CalcPosition(double jdtt)
    {
        // Get the Earth's heliocentric position.
        var earth = earthService.GetPlanet();
        (double lngEarth, double latEarth, double R_m) =
            planetService.CalcPlanetPosition(earth, jdtt);

        // Reverse to get the mean dynamical ecliptic and equinox of the date.
        double lngSun = Angle.NormalizeRadians(lngEarth + PI);
        double latSun = -latEarth;

        // Convert to FK5.
        // This gives the true ("geometric") longitude of the Sun referred to the
        // mean equinox of the date.
        double julCen = TimeScaleService.JulianCenturiesSinceJ2000(jdtt);
        double lambdaPrime = lngSun - Angle.DegToRad(1.397) * julCen
            - Angle.DegToRad(0.000_31) * julCen * julCen;
        lngSun -= Angle.DmsToRad(0, 0, 0.090_33);
        latSun += Angle.DmsToRad(0, 0, 0.039_16) * (Cos(lambdaPrime) - Sin(lambdaPrime));

        // TODO
        // To obtain the apparent longitude, nutation and aberration have to be
        // taken into account.
        // SofaLibrary.iauNut06a(jdtt, 0, out double dpsi, out double deps);
        // lngSun += dpsi;

        // Calculate and add aberration.
        double julMil = julCen / 10;
        double julMil2 = julMil * julMil;
        double dLambda_as = 3548.193
            + 118.568 * Angle.SinDeg(87.5287 + 359_993.7286 * julMil)
            + 2.476 * Angle.SinDeg(85.0561 + 719_987.4571 * julMil)
            + 1.376 * Angle.SinDeg(27.8502 + 4452_671.1152 * julMil)
            + 0.119 * Angle.SinDeg(73.1375 + 450_368.8564 * julMil)
            + 0.114 * Angle.SinDeg(337.2264 + 329_644.6718 * julMil)
            + 0.086 * Angle.SinDeg(222.5400 + 659_289.3436 * julMil)
            + 0.078 * Angle.SinDeg(162.8136 + 9224_659.7915 * julMil)
            + 0.054 * Angle.SinDeg(82.5823 + 1079_981.1857 * julMil)
            + 0.052 * Angle.SinDeg(171.5189 + 225_184.4282 * julMil)
            + 0.034 * Angle.SinDeg(30.3214 + 4092_677.3866 * julMil)
            + 0.033 * Angle.SinDeg(119.8105 + 337_181.4711 * julMil)
            + 0.023 * Angle.SinDeg(247.5418 + 299_295.6151 * julMil)
            + 0.023 * Angle.SinDeg(325.1526 + 315_559.5560 * julMil)
            + 0.021 * Angle.SinDeg(155.1241 + 675_553.2846 * julMil)
            + 7.311 * julMil * Angle.SinDeg(333.4515 + 359_993.7286 * julMil)
            + 0.305 * julMil * Angle.SinDeg(330.9814 + 719_987.4571 * julMil)
            + 0.010 * julMil * Angle.SinDeg(328.5170 + 1079_981.1857 * julMil)
            + 0.309 * julMil2 * Angle.SinDeg(241.4518 + 359_993.7286 * julMil)
            + 0.021 * julMil2 * Angle.SinDeg(205.0482 + 719_987.4571 * julMil)
            + 0.004 * julMil2 * Angle.SinDeg(297.8610 + 4452_671.1152 * julMil)
            + 0.010 * julMil2 * Angle.SinDeg(154.7066 + 359_993.7286 * julMil);
        double dLambda_rad = dLambda_as * Angle.RadiansPerArcsecond;
        double R_AU = R_m / Length.MetresPerAu;
        double aberration = -0.005_775_518 * R_AU * dLambda_rad;
        lngSun += aberration;

        return (lngSun, latSun);
    }

    /// <summary>
    /// Calculate apparent solar longitude for a given instant specified as a
    /// DateTime (UT).
    /// </summary>
    /// <param name="dt">The instant specified as a DateTime (UT).</param>
    /// <returns>The longitude and latitude of the Sun, in radians, at the given
    /// instant.</returns>
    public (double Lng, double Lat) CalcPosition(DateTime dt)
    {
        return CalcPosition(TimeScaleService.DateTimeToJulianDateTerrestrial(dt));
    }
}
