using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Galaxon.Astronomy.Repository;

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

    public DbSet<Star> Stars => Set<Star>();

    public DbSet<Planet> Planets => Set<Planet>();

    public DbSet<Planetoid> Planetoids => Set<Planetoid>();

    public DbSet<Satellite> Satellites => Set<Satellite>();

    /// <summary>
    /// AstroObject components.
    /// </summary>
    public DbSet<Physical> PhysicalParams => Set<Physical>();

    public DbSet<Rotation> RotationalParams => Set<Rotation>();

    public DbSet<Orbit> OrbitalParams => Set<Orbit>();

    public DbSet<Observation> ObservationalParams => Set<Observation>();

    public DbSet<Stellar> StellarParams => Set<Stellar>();

    public DbSet<Atmosphere> Atmospheres => Set<Atmosphere>();

    public DbSet<AtmoConstituent> AtmoConstituents => Set<AtmoConstituent>();

    public DbSet<Molecule> Molecules => Set<Molecule>();

    public DbSet<MinorPlanetRecord> MinorPlanetRecords => Set<MinorPlanetRecord>();

    /// <summary>
    /// Other stuff.
    /// </summary>
    public DbSet<SeasonalMarker> SeasonalMarkers => Set<SeasonalMarker>();

    public DbSet<LunarPhase> LunarPhases => Set<LunarPhase>();

    public DbSet<EasterDate> EasterDates => Set<EasterDate>();

    public DbSet<DeltaTEntry> DeltaTEntries => Set<DeltaTEntry>();

    public DbSet<VSOP87DRecord> VSOP87D => Set<VSOP87DRecord>();

    public DbSet<LeapSecond> LeapSeconds => Set<LeapSecond>();

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
            .HasForeignKey<Physical>(phys => phys.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Rotation)
            .WithOne(rot => rot.AstroObject)
            .HasForeignKey<Rotation>(rot => rot.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Orbit)
            .WithOne(orb => orb.AstroObject)
            .HasForeignKey<Orbit>(orb => orb.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Observation)
            .WithOne(obs => obs.AstroObject)
            .HasForeignKey<Observation>(obs => obs.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Atmosphere)
            .WithOne(atmo => atmo.AstroObject)
            .HasForeignKey<Atmosphere>(atmo => atmo.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.Stellar)
            .WithOne(ss => ss.AstroObject)
            .HasForeignKey<Stellar>(ss => ss.AstroObjectId);
        builder.Entity<AstroObject>()
            .HasOne(ao => ao.MinorPlanetRecord)
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
