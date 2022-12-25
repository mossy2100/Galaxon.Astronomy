using System.Globalization;
using Galaxon.Core.Strings;
using Galaxon.Core.Time;
using CsvHelper;

namespace Galaxon.Astronomy.Repository;

public class Planet : AstroObject
{
    #region Static Properties

    /// <summary>
    /// I'll hard code this for now. If we find any new planets (or Pluto
    /// gets reclassified again), we can update it.
    /// </summary>
    public const byte Count = 8;

    /// <summary>
    /// Get all the planets. There are only 8, so this is a useful and
    /// efficient cache.
    /// </summary>
    private static List<Planet> s_all = new();

    public static List<Planet> All
    {
        get
        {
            // If we didn't load them yet, do it now.
            if (s_all.Count == 0)
            {
                using AstroDbContext db = new();
                s_all = db.Planets.ToList();
            }
            return s_all;
        }
    }

    #endregion Static Properties

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Planet()
    {
    }

    /// <summary>
    /// Get the planet name given a number.
    /// </summary>
    /// <param name="num">The planet number as used in VSOP87 data.</param>
    /// <returns>The planet name.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the number is not in
    /// the range 1..8.</exception>
    public static string NumberToName(byte num) =>
        num switch
        {
            1 => "Mercury",
            2 => "Venus",
            3 => "Earth",
            4 => "Mars",
            5 => "Jupiter",
            6 => "Saturn",
            7 => "Uranus",
            8 => "Neptune",
            _ => throw new ArgumentOutOfRangeException(
                $"Planet number must be in the range 1..{Count}.")
        };

    /// <summary>
    /// Load a planet from the database by name.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Planet? Load(AstroDbContext db, string name) =>
        AstroObject.Load(db.Planets, name);

    /// <summary>
    /// Load a planet from the database by number.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public static Planet? Load(AstroDbContext db, uint num) =>
        AstroObject.Load(db.Planets, num);

    /// <summary>
    /// Initialize the planet data from the CSV file.
    /// </summary>
    public static void InitializeData()
    {
        using AstroDbContext db = new();

        // Get the Sun.
        Star? sun = Star.Load(db, "Sun");

        // Open the CSV file for parsing.
        string csvPath = $"{AstroDbContext.DataDirectory()}/Planets/Planets.csv";
        using StreamReader stream = new(csvPath);
        using CsvReader csv = new(stream, CultureInfo.InvariantCulture);
        // Skip the header row.
        csv.Read();
        csv.ReadHeader();

        // Create or update the planet records.
        while (csv.Read())
        {
            string? name = csv.GetField(0);
            string? type = csv.GetField(2);

            if (name == null)
            {
                continue;
            }

            Planet? planet = Load(db.Planets, name);

            if (planet == null)
            {
                // Create a new planet.
                Console.WriteLine($"Adding new planet {name}.");
                planet = new Planet
                {
                    Name = name
                };
                db.Planets.Add(planet);
            }
            else
            {
                Console.WriteLine($"Updating planet {name}.");
                db.Planets.Attach(planet);
            }

            // Set the planet's basic parameters.

            // Set its number.
            string? num = csv.GetField(1);
            planet.Number = num == null ? 0 : uint.Parse(num);

            // Set its groups.
            planet.AddToGroup(db, "Planet");
            planet.AddToGroup(db, $"{type} planet");

            // Set its parent.
            planet.Parent = sun;

            // Save the planet object now to ensure it has an Id before
            // attaching composition objects to it.
            db.SaveChanges();

            // Orbital parameters.
            // TODO Fix this. The Orbit and other objects aren't loaded by
            // TODO default now when the planet is loaded, so we have to look in
            // TODO the database for the Orbit object first, before creating a new one.
            planet.Orbit ??= new Orbit();

            // Apoapsis is provided in km, convert to m.
            const double kilo = 1000;
            double? apoapsis = csv.GetField(3)?.ToDouble();
            planet.Orbit.Apoapsis = Quantity.ConvertAmount(apoapsis, "km", "m");

            // Periapsis is provided in km, convert to m.
            double? periapsis = csv.GetField(4)?.ToDouble();
            planet.Orbit.Periapsis = Quantity.ConvertAmount(periapsis, "km", "m");

            // Semi-major axis is provided in km, convert to m.
            planet.Orbit.SemiMajorAxis = csv.GetField(5)?.ToDouble() * kilo;
            planet.Orbit.Eccentricity = csv.GetField(6)?.ToDouble();

            // Sidereal orbit period is provided in days, convert to seconds.
            planet.Orbit.SiderealOrbitPeriod = csv.GetField(7)?.ToDouble() * XTimeSpan.SecondsPerDay;

            // Synodic orbit period is provided in days, convert to seconds.
            double? synodicPeriod = csv.GetField(8)?.ToDouble();
            planet.Orbit.SynodicOrbitPeriod = synodicPeriod * XTimeSpan.SecondsPerDay;

            // Avg orbital speed is provided in km/s, convert to m/s.
            planet.Orbit.AvgOrbitSpeed = csv.GetField(9)?.ToDouble() * kilo;

            // Mean anomaly is provided in degrees, convert to radians.
            planet.Orbit.MeanAnomaly = csv.GetField(10)?.ToDouble() * Angle.RadiansPerDegree;

            // Inclination is provided in degrees, convert to radians.
            planet.Orbit.Inclination = csv.GetField(11)?.ToDouble() * Angle.RadiansPerDegree;

            // Long. of asc. node is provided in degrees, convert to radians.
            planet.Orbit.LongAscNode = csv.GetField(12)?.ToDouble() * Angle.RadiansPerDegree;

            // Arg. of perihelion is provided in degrees, convert to radians.
            planet.Orbit.ArgPeriapsis = csv.GetField(13)?.ToDouble() * Angle.RadiansPerDegree;

            // Calculate the mean motion in rad/s.
            planet.Orbit.MeanMotion = Tau / planet.Orbit.SiderealOrbitPeriod;

            // Save the orbital parameters.
            db.SaveChanges();

            // Physical parameters.
            planet.Physical ??= new Physical();
            double? equatRadius = csv.GetField(15)?.ToDouble() * kilo;
            double? polarRadius = csv.GetField(16)?.ToDouble() * kilo;
            planet.Physical.SetSpheroid(equatRadius ?? 0, polarRadius ?? 0);
            planet.Physical.MeanRadius = csv.GetField(14)?.ToDouble() * kilo;
            planet.Physical.Flattening = csv.GetField(17)?.ToDouble();
            planet.Physical.SurfaceArea = csv.GetField(18)?.ToDouble() * kilo * kilo;
            planet.Physical.Volume = csv.GetField(19)?.ToDouble() * Pow(kilo, 3);
            planet.Physical.Mass = csv.GetField(20)?.ToDouble();
            planet.Physical.Density =
                Density.GramsPerCm3ToKgPerM3(csv.GetField(21)?.ToDouble() ?? 0);
            planet.Physical.SurfaceGrav = csv.GetField(22)?.ToDouble();
            planet.Physical.MomentOfInertiaFactor = csv.GetField(23)?.ToDouble();
            planet.Physical.EscapeVelocity = csv.GetField(24)?.ToDouble() * kilo;
            planet.Physical.StdGravParam = csv.GetField(25)?.ToDouble();
            planet.Physical.GeometricAlbedo = csv.GetField(26)?.ToDouble();
            planet.Physical.SolarIrradiance = csv.GetField(27)?.ToDouble();
            planet.Physical.HasGlobalMagField = csv.GetField(28) == "Y";
            planet.Physical.HasRingSystem = csv.GetField(29) == "Y";
            planet.Physical.MinSurfaceTemp = csv.GetField(36)?.ToDouble();
            planet.Physical.MeanSurfaceTemp = csv.GetField(37)?.ToDouble();
            planet.Physical.MaxSurfaceTemp = csv.GetField(38)?.ToDouble();
            db.SaveChanges();

            // Rotational parameters.
            planet.Rotation ??= new Rotation();
            planet.Rotation.SynodicRotationPeriod =
                csv.GetField(30)?.ToDouble() * XTimeSpan.SecondsPerDay;
            planet.Rotation.SiderealRotationPeriod =
                csv.GetField(31)?.ToDouble() * XTimeSpan.SecondsPerDay;
            planet.Rotation.EquatRotationVelocity = csv.GetField(32)?.ToDouble();
            planet.Rotation.Obliquity = csv.GetField(33)?.ToDouble() * Angle.RadiansPerDegree;
            planet.Rotation.NorthPoleRightAscension =
                csv.GetField(34)?.ToDouble() * Angle.RadiansPerDegree;
            planet.Rotation.NorthPoleDeclination =
                csv.GetField(35)?.ToDouble() * Angle.RadiansPerDegree;
            db.SaveChanges();

            // Atmosphere.
            planet.Atmosphere ??= new Atmosphere();
            planet.Atmosphere.SurfacePressure = csv.GetField(40)?.ToDouble();
            planet.Atmosphere.ScaleHeight = csv.GetField(41)?.ToDouble() * kilo;
            planet.Atmosphere.IsSurfaceBoundedExosphere = name == "Mercury";
            db.SaveChanges();
        }
    }
}
