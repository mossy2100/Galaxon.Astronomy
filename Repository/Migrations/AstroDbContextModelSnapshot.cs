﻿// <auto-generated />
using System;
using Galaxon.Astronomy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Galaxon.Astronomy.Repository.Migrations
{
    [DbContext(typeof(AstroDbContext))]
    partial class AstroDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Galaxon.Astronomy.AstroObject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("Number")
                        .HasColumnType("int");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("AstroObjects");

                    b.HasDiscriminator<string>("Type").HasValue("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.AstroObjectGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("AstroObjectGroups");
                });

            modelBuilder.Entity("Galaxon.Astronomy.AtmoConstituent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AtmosphereId")
                        .HasColumnType("int");

                    b.Property<int>("MoleculeId")
                        .HasColumnType("int");

                    b.Property<double?>("Percentage")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AtmosphereId");

                    b.HasIndex("MoleculeId");

                    b.ToTable("AtmoConstituents");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Atmosphere", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<bool?>("IsSurfaceBoundedExosphere")
                        .HasColumnType("bit");

                    b.Property<double?>("ScaleHeight")
                        .HasColumnType("float");

                    b.Property<double?>("SurfacePressure")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("Atmospheres");
                });

            modelBuilder.Entity("Galaxon.Astronomy.DeltaTEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("DeltaT")
                        .HasColumnType("float");

                    b.Property<short>("Year")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.ToTable("DeltaTEntries");
                });

            modelBuilder.Entity("Galaxon.Astronomy.EasterDate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("EasterDates");
                });

            modelBuilder.Entity("Galaxon.Astronomy.LeapSecond", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("LeapSeconds");
                });

            modelBuilder.Entity("Galaxon.Astronomy.LunarPhase", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte>("PhaseNumber")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("UtcDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("LunarPhases");
                });

            modelBuilder.Entity("Galaxon.Astronomy.MinorPlanetRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<int?>("CoOrbitalObjectId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DatePeriapsis")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("Is1Opp")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsCriticalListNumberedObject")
                        .HasColumnType("bit");

                    b.Property<byte?>("OrbitType")
                        .HasColumnType("tinyint");

                    b.Property<string>("PackedDesignation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReadableDesignation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SMASS")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tholen")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.HasIndex("CoOrbitalObjectId");

                    b.ToTable("MinorPlanetRecords");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Molecule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Molecules");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Observation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double?>("AbsMag")
                        .HasColumnType("float");

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<double?>("MaxAngularDiam")
                        .HasColumnType("float");

                    b.Property<double?>("MaxApparentMag")
                        .HasColumnType("float");

                    b.Property<double?>("MinAngularDiam")
                        .HasColumnType("float");

                    b.Property<double?>("MinApparentMag")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("ObservationalParams");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Orbit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double?>("Apoapsis")
                        .HasColumnType("float");

                    b.Property<double?>("ArgPeriapsis")
                        .HasColumnType("float");

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<double?>("AvgOrbitSpeed")
                        .HasColumnType("float");

                    b.Property<double?>("Eccentricity")
                        .HasColumnType("float");

                    b.Property<DateTime?>("Epoch")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Inclination")
                        .HasColumnType("float");

                    b.Property<double?>("LongAscNode")
                        .HasColumnType("float");

                    b.Property<double?>("MeanAnomaly")
                        .HasColumnType("float");

                    b.Property<double?>("MeanMotion")
                        .HasColumnType("float");

                    b.Property<double?>("Periapsis")
                        .HasColumnType("float");

                    b.Property<double?>("SemiMajorAxis")
                        .HasColumnType("float");

                    b.Property<double?>("SiderealOrbitPeriod")
                        .HasColumnType("float");

                    b.Property<double?>("SynodicOrbitPeriod")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("OrbitalParams");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Physical", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<double?>("ColorBV")
                        .HasColumnType("float");

                    b.Property<double?>("ColorUB")
                        .HasColumnType("float");

                    b.Property<double?>("Density")
                        .HasColumnType("float");

                    b.Property<double?>("EscapeVelocity")
                        .HasColumnType("float");

                    b.Property<double?>("Flattening")
                        .HasColumnType("float");

                    b.Property<double?>("GeometricAlbedo")
                        .HasColumnType("float");

                    b.Property<bool?>("HasGlobalMagField")
                        .HasColumnType("bit");

                    b.Property<bool?>("HasRingSystem")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsRound")
                        .HasColumnType("bit");

                    b.Property<double?>("Mass")
                        .HasColumnType("float");

                    b.Property<double?>("MaxSurfaceTemp")
                        .HasColumnType("float");

                    b.Property<double?>("MeanRadius")
                        .HasColumnType("float");

                    b.Property<double?>("MeanSurfaceTemp")
                        .HasColumnType("float");

                    b.Property<double?>("MinSurfaceTemp")
                        .HasColumnType("float");

                    b.Property<double?>("MomentOfInertiaFactor")
                        .HasColumnType("float");

                    b.Property<double?>("RadiusA")
                        .HasColumnType("float");

                    b.Property<double?>("RadiusB")
                        .HasColumnType("float");

                    b.Property<double?>("RadiusC")
                        .HasColumnType("float");

                    b.Property<double?>("SolarIrradiance")
                        .HasColumnType("float");

                    b.Property<double?>("StdGravParam")
                        .HasColumnType("float");

                    b.Property<double?>("SurfaceArea")
                        .HasColumnType("float");

                    b.Property<double?>("SurfaceGrav")
                        .HasColumnType("float");

                    b.Property<double?>("Volume")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("PhysicalParams");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Rotation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<double?>("EquatRotationVelocity")
                        .HasColumnType("float");

                    b.Property<double?>("NorthPoleDeclination")
                        .HasColumnType("float");

                    b.Property<double?>("NorthPoleRightAscension")
                        .HasColumnType("float");

                    b.Property<double?>("Obliquity")
                        .HasColumnType("float");

                    b.Property<double?>("SiderealRotationPeriod")
                        .HasColumnType("float");

                    b.Property<double?>("SynodicRotationPeriod")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("RotationalParams");
                });

            modelBuilder.Entity("Galaxon.Astronomy.SeasonalMarker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<byte>("MarkerNumber")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("UtcDateTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("SeasonalMarkers");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Stellar", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<double?>("Luminosity")
                        .HasColumnType("float");

                    b.Property<double?>("Metallicity")
                        .HasColumnType("float");

                    b.Property<double?>("Radiance")
                        .HasColumnType("float");

                    b.Property<string>("SpectralClass")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId")
                        .IsUnique();

                    b.ToTable("StellarParams");
                });

            modelBuilder.Entity("Galaxon.Astronomy.VSOP87DRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("Amplitude")
                        .HasColumnType("float");

                    b.Property<int>("AstroObjectId")
                        .HasColumnType("int");

                    b.Property<byte>("Exponent")
                        .HasColumnType("tinyint");

                    b.Property<double>("Frequency")
                        .HasColumnType("float");

                    b.Property<short>("Index")
                        .HasColumnType("smallint");

                    b.Property<double>("Phase")
                        .HasColumnType("float");

                    b.Property<string>("PlanetName")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Variable")
                        .IsRequired()
                        .HasColumnType("char(1)");

                    b.HasKey("Id");

                    b.HasIndex("AstroObjectId");

                    b.ToTable("VSOP87D");
                });

            modelBuilder.Entity("AstroObjectAstroObjectGroup", b =>
                {
                    b.Property<int>("GroupsId")
                        .HasColumnType("int");

                    b.Property<int>("ObjectsId")
                        .HasColumnType("int");

                    b.HasKey("GroupsId", "ObjectsId");

                    b.HasIndex("ObjectsId");

                    b.ToTable("AstroObjectAstroObjectGroup");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Planet", b =>
                {
                    b.HasBaseType("Galaxon.Astronomy.AstroObject");

                    b.HasDiscriminator().HasValue("Planet");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Planetoid", b =>
                {
                    b.HasBaseType("Galaxon.Astronomy.AstroObject");

                    b.HasDiscriminator().HasValue("Planetoid");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Satellite", b =>
                {
                    b.HasBaseType("Galaxon.Astronomy.AstroObject");

                    b.HasDiscriminator().HasValue("Satellite");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Star", b =>
                {
                    b.HasBaseType("Galaxon.Astronomy.AstroObject");

                    b.HasDiscriminator().HasValue("Star");
                });

            modelBuilder.Entity("Galaxon.Astronomy.AstroObject", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Galaxon.Astronomy.AstroObjectGroup", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObjectGroup", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Galaxon.Astronomy.AtmoConstituent", b =>
                {
                    b.HasOne("Galaxon.Astronomy.Atmosphere", "Atmosphere")
                        .WithMany("Constituents")
                        .HasForeignKey("AtmosphereId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Galaxon.Astronomy.Molecule", "Molecule")
                        .WithMany()
                        .HasForeignKey("MoleculeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Atmosphere");

                    b.Navigation("Molecule");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Atmosphere", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Atmosphere")
                        .HasForeignKey("Galaxon.Astronomy.Atmosphere", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.MinorPlanetRecord", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("MinorPlanetRecord")
                        .HasForeignKey("Galaxon.Astronomy.MinorPlanetRecord", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Galaxon.Astronomy.AstroObject", "CoOrbitalObject")
                        .WithMany()
                        .HasForeignKey("CoOrbitalObjectId");

                    b.Navigation("AstroObject");

                    b.Navigation("CoOrbitalObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Observation", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Observation")
                        .HasForeignKey("Galaxon.Astronomy.Observation", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Orbit", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Orbit")
                        .HasForeignKey("Galaxon.Astronomy.Orbit", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Physical", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Physical")
                        .HasForeignKey("Galaxon.Astronomy.Physical", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Rotation", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Rotation")
                        .HasForeignKey("Galaxon.Astronomy.Rotation", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Stellar", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithOne("Stellar")
                        .HasForeignKey("Galaxon.Astronomy.Stellar", "AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("Galaxon.Astronomy.VSOP87DRecord", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObject", "AstroObject")
                        .WithMany("VSOP87DRecords")
                        .HasForeignKey("AstroObjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AstroObject");
                });

            modelBuilder.Entity("AstroObjectAstroObjectGroup", b =>
                {
                    b.HasOne("Galaxon.Astronomy.AstroObjectGroup", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Galaxon.Astronomy.AstroObject", null)
                        .WithMany()
                        .HasForeignKey("ObjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Galaxon.Astronomy.AstroObject", b =>
                {
                    b.Navigation("Atmosphere");

                    b.Navigation("Children");

                    b.Navigation("MinorPlanetRecord");

                    b.Navigation("Observation");

                    b.Navigation("Orbit");

                    b.Navigation("Physical");

                    b.Navigation("Rotation");

                    b.Navigation("Stellar");

                    b.Navigation("VSOP87DRecords");
                });

            modelBuilder.Entity("Galaxon.Astronomy.Atmosphere", b =>
                {
                    b.Navigation("Constituents");
                });
#pragma warning restore 612, 618
        }
    }
}
