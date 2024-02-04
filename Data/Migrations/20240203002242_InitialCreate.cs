using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Galaxon.Astronomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AstroObjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstroObjectGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AstroObjectGroups_AstroObjectGroups_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AstroObjectGroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AstroObjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Number = table.Column<int>(type: "int", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstroObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AstroObjects_AstroObjects_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DeltaTRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    DeltaT = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeltaTRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EasterDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasterDates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeapSeconds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IersBulletinCDate = table.Column<DateTime>(type: "date", nullable: false),
                    Value = table.Column<short>(type: "smallint", nullable: false),
                    LeapSecondDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeapSeconds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LunarPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhaseNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    UtcDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LunarPhases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Molecules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Molecules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeasonalMarkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarkerNumber = table.Column<byte>(type: "tinyint", nullable: false),
                    UtcDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonalMarkers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AstroObjectAstroObjectGroup",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "int", nullable: false),
                    ObjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstroObjectAstroObjectGroup", x => new { x.GroupsId, x.ObjectsId });
                    table.ForeignKey(
                        name: "FK_AstroObjectAstroObjectGroup_AstroObjectGroups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "AstroObjectGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AstroObjectAstroObjectGroup_AstroObjects_ObjectsId",
                        column: x => x.ObjectsId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtmosphereRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    SurfacePressure = table.Column<double>(type: "float", nullable: true),
                    ScaleHeight = table.Column<double>(type: "float", nullable: true),
                    IsSurfaceBoundedExosphere = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtmosphereRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtmosphereRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObservationalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    AbsMag = table.Column<double>(type: "float", nullable: true),
                    MinApparentMag = table.Column<double>(type: "float", nullable: true),
                    MaxApparentMag = table.Column<double>(type: "float", nullable: true),
                    MinAngularDiam = table.Column<double>(type: "float", nullable: true),
                    MaxAngularDiam = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObservationalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationalRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    Epoch = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Eccentricity = table.Column<double>(type: "float", nullable: true),
                    SemiMajorAxis = table.Column<double>(type: "float", nullable: true),
                    Inclination = table.Column<double>(type: "float", nullable: true),
                    LongAscNode = table.Column<double>(type: "float", nullable: true),
                    ArgPeriapsis = table.Column<double>(type: "float", nullable: true),
                    MeanAnomaly = table.Column<double>(type: "float", nullable: true),
                    Apoapsis = table.Column<double>(type: "float", nullable: true),
                    Periapsis = table.Column<double>(type: "float", nullable: true),
                    SiderealOrbitPeriod = table.Column<double>(type: "float", nullable: true),
                    SynodicOrbitPeriod = table.Column<double>(type: "float", nullable: true),
                    AvgOrbitSpeed = table.Column<double>(type: "float", nullable: true),
                    MeanMotion = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrbitalRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    RadiusA = table.Column<double>(type: "float", nullable: true),
                    RadiusB = table.Column<double>(type: "float", nullable: true),
                    RadiusC = table.Column<double>(type: "float", nullable: true),
                    IsRound = table.Column<bool>(type: "bit", nullable: true),
                    MeanRadius = table.Column<double>(type: "float", nullable: true),
                    Flattening = table.Column<double>(type: "float", nullable: true),
                    SurfaceArea = table.Column<double>(type: "float", nullable: true),
                    Volume = table.Column<double>(type: "float", nullable: true),
                    Mass = table.Column<double>(type: "float", nullable: true),
                    Density = table.Column<double>(type: "float", nullable: true),
                    SurfaceGrav = table.Column<double>(type: "float", nullable: true),
                    EscapeVelocity = table.Column<double>(type: "float", nullable: true),
                    StdGravParam = table.Column<double>(type: "float", nullable: true),
                    MomentOfInertiaFactor = table.Column<double>(type: "float", nullable: true),
                    HasGlobalMagField = table.Column<bool>(type: "bit", nullable: true),
                    HasRingSystem = table.Column<bool>(type: "bit", nullable: true),
                    SolarIrradiance = table.Column<double>(type: "float", nullable: true),
                    GeometricAlbedo = table.Column<double>(type: "float", nullable: true),
                    ColorBV = table.Column<double>(type: "float", nullable: true),
                    ColorUB = table.Column<double>(type: "float", nullable: true),
                    MinSurfaceTemp = table.Column<double>(type: "float", nullable: true),
                    MeanSurfaceTemp = table.Column<double>(type: "float", nullable: true),
                    MaxSurfaceTemp = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotationalRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    SiderealRotationPeriod = table.Column<double>(type: "float", nullable: true),
                    SynodicRotationPeriod = table.Column<double>(type: "float", nullable: true),
                    EquatRotationVelocity = table.Column<double>(type: "float", nullable: true),
                    Obliquity = table.Column<double>(type: "float", nullable: true),
                    NorthPoleRightAscension = table.Column<double>(type: "float", nullable: true),
                    NorthPoleDeclination = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RotationalRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationalRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StellarRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    SpectralClass = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Metallicity = table.Column<double>(type: "float", nullable: true),
                    Luminosity = table.Column<double>(type: "float", nullable: true),
                    Radiance = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StellarRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StellarRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VSOP87DRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanetName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    Variable = table.Column<string>(type: "char(1)", nullable: false),
                    Exponent = table.Column<byte>(type: "tinyint", nullable: false),
                    Index = table.Column<short>(type: "smallint", nullable: false),
                    Amplitude = table.Column<double>(type: "float", nullable: false),
                    Phase = table.Column<double>(type: "float", nullable: false),
                    Frequency = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VSOP87DRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VSOP87DRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtmosphereConstituents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AtmosphereId = table.Column<int>(type: "int", nullable: false),
                    MoleculeId = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtmosphereConstituents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtmosphereConstituents_AtmosphereRecords_AtmosphereId",
                        column: x => x.AtmosphereId,
                        principalTable: "AtmosphereRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtmosphereConstituents_Molecules_MoleculeId",
                        column: x => x.MoleculeId,
                        principalTable: "Molecules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AstroObjectAstroObjectGroup_ObjectsId",
                table: "AstroObjectAstroObjectGroup",
                column: "ObjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_AstroObjectGroups_ParentId",
                table: "AstroObjectGroups",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AstroObjects_ParentId",
                table: "AstroObjects",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AtmosphereConstituents_AtmosphereId",
                table: "AtmosphereConstituents",
                column: "AtmosphereId");

            migrationBuilder.CreateIndex(
                name: "IX_AtmosphereConstituents_MoleculeId",
                table: "AtmosphereConstituents",
                column: "MoleculeId");

            migrationBuilder.CreateIndex(
                name: "IX_AtmosphereRecords_AstroObjectId",
                table: "AtmosphereRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObservationalRecords_AstroObjectId",
                table: "ObservationalRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalRecords_AstroObjectId",
                table: "OrbitalRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalRecords_AstroObjectId",
                table: "PhysicalRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RotationalRecords_AstroObjectId",
                table: "RotationalRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StellarRecords_AstroObjectId",
                table: "StellarRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VSOP87DRecords_AstroObjectId",
                table: "VSOP87DRecords",
                column: "AstroObjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AstroObjectAstroObjectGroup");

            migrationBuilder.DropTable(
                name: "AtmosphereConstituents");

            migrationBuilder.DropTable(
                name: "DeltaTRecords");

            migrationBuilder.DropTable(
                name: "EasterDates");

            migrationBuilder.DropTable(
                name: "LeapSeconds");

            migrationBuilder.DropTable(
                name: "LunarPhases");

            migrationBuilder.DropTable(
                name: "ObservationalRecords");

            migrationBuilder.DropTable(
                name: "OrbitalRecords");

            migrationBuilder.DropTable(
                name: "PhysicalRecords");

            migrationBuilder.DropTable(
                name: "RotationalRecords");

            migrationBuilder.DropTable(
                name: "SeasonalMarkers");

            migrationBuilder.DropTable(
                name: "StellarRecords");

            migrationBuilder.DropTable(
                name: "VSOP87DRecords");

            migrationBuilder.DropTable(
                name: "AstroObjectGroups");

            migrationBuilder.DropTable(
                name: "AtmosphereRecords");

            migrationBuilder.DropTable(
                name: "Molecules");

            migrationBuilder.DropTable(
                name: "AstroObjects");
        }
    }
}
