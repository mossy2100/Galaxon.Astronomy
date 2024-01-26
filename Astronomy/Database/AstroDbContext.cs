using Galaxon.Astronomy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Galaxon.Astronomy.Database;

/// <summary>
/// Handles the database connection and operations.
/// </summary>
public class AstroDbContext : DbContext
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public AstroDbContext() { }

    /// <summary>
    /// AstroObjects types.
    /// </summary>
    public DbSet<AstroObject> AstroObjects => Set<AstroObject>();

    public DbSet<AstroObjectGroup> AstroObjectGroups => Set<AstroObjectGroup>();

    // public DbSet<Star> Stars => Set<Star>();
    //
    // public DbSet<Planet> Planets => Set<Planet>();
    //
    // public DbSet<Planetoid> Planetoids => Set<Planetoid>();
    //
    // public DbSet<Moon> Moons => Set<Moon>();

    /// <summary>
    /// AstroObject components.
    /// </summary>
    public DbSet<PhysicalRecord> PhysicalRecords => Set<PhysicalRecord>();

    public DbSet<RotationalRecord> RotationalRecords => Set<RotationalRecord>();

    public DbSet<OrbitalRecord> OrbitalRecords => Set<OrbitalRecord>();

    public DbSet<ObservationalRecord> ObservationalRecords => Set<ObservationalRecord>();

    public DbSet<StellarRecord> StellarRecords => Set<StellarRecord>();

    public DbSet<AtmosphereRecord> AtmosphereRecords => Set<AtmosphereRecord>();

    /// <summary>
    /// Atmosphere constituents.
    /// </summary>
    public DbSet<AtmosphereConstituent> AtmosphereConstituents => Set<AtmosphereConstituent>();

    public DbSet<Molecule> Molecules => Set<Molecule>();

    /// <summary>
    /// Other stuff.
    /// </summary>
    public DbSet<SeasonalMarker> SeasonalMarkers => Set<SeasonalMarker>();

    public DbSet<LunarPhase> LunarPhases => Set<LunarPhase>();

    public DbSet<EasterDate> EasterDates => Set<EasterDate>();

    public DbSet<DeltaTRecord> DeltaTRecords => Set<DeltaTRecord>();

    public DbSet<VSOP87DRecord> VSOP87DRecords => Set<VSOP87DRecord>();

    public DbSet<LeapSecond> LeapSeconds => Set<LeapSecond>();

    public DbSet<MinorPlanetRecord> MinorPlanetRecords => Set<MinorPlanetRecord>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer(
                "Server=localhost,1433; Database=TimeAndSpace; User Id=SA; Password=$HappyHealthyRich$; Encrypt=False",
                options => options.EnableRetryOnFailure())
            .LogTo(Console.WriteLine, new[]
            {
                DbLoggerCategory.Database.Command.Name
            }, LogLevel.Information)
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AstroObject>()
            .HasDiscriminator<string>("Type");
        builder.Entity<AstroObject>()
            .HasMany(ao => ao.Groups)
            .WithMany(g => g.Objects);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Parent)
            .WithMany(ao => ao.Children);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Physical)
            .WithOne(phys => phys.AstroObject)
            .HasForeignKey<PhysicalRecord>(phys => phys.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Rotation)
            .WithOne(rot => rot.AstroObject)
            .HasForeignKey<RotationalRecord>(rot => rot.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Orbit)
            .WithOne(orb => orb.AstroObject)
            .HasForeignKey<OrbitalRecord>(orb => orb.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Observation)
            .WithOne(obs => obs.AstroObject)
            .HasForeignKey<ObservationalRecord>(obs => obs.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Atmosphere)
            .WithOne(atmo => atmo.AstroObject)
            .HasForeignKey<AtmosphereRecord>(atmo => atmo.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Stellar)
            .WithOne(ss => ss.AstroObject)
            .HasForeignKey<StellarRecord>(ss => ss.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.MinorPlanet)
            .WithOne(mpr => mpr.AstroObject)
            .HasForeignKey<MinorPlanetRecord>(mpr => mpr.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasMany(ao => ao.VSOP87DRecords)
            .WithOne(vr => vr.AstroObject)
            .HasForeignKey(vr => vr.AstroObjectId);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>();
        configurationBuilder
            .Properties<DateTime>()
            .HaveConversion<DateTimeConverter>();
    }

    public static string DataDirectory()
    {
        return @"/Users/shaun/Documents/Web & software development/C#/Projects/Data";
    }
}
