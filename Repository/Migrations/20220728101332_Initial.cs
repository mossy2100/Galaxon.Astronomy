using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Astronomy.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AstroObjectGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Name = table.Column<string>(type: "varchar(50)", nullable: true),
                    Number = table.Column<int>(type: "int", nullable: true),
                    Type = table.Column<string>(type: "varchar(20)", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true)
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
                name: "DeltaTEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    DeltaT = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeltaTEntries", x => x.Id);
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
                    Date = table.Column<DateTime>(type: "date", nullable: false)
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
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "Atmospheres",
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
                    table.PrimaryKey("PK_Atmospheres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Atmospheres_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MinorPlanetRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    PackedDesignation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReadableDesignation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePeriapsis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrbitType = table.Column<byte>(type: "tinyint", nullable: true),
                    Is1Opp = table.Column<bool>(type: "bit", nullable: true),
                    IsCriticalListNumberedObject = table.Column<bool>(type: "bit", nullable: true),
                    CoOrbitalObjectId = table.Column<int>(type: "int", nullable: true),
                    Tholen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SMASS = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MinorPlanetRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MinorPlanetRecords_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MinorPlanetRecords_AstroObjects_CoOrbitalObjectId",
                        column: x => x.CoOrbitalObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ObservationalParams",
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
                    table.PrimaryKey("PK_ObservationalParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObservationalParams_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbitalParams",
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
                    table.PrimaryKey("PK_OrbitalParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrbitalParams_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalParams",
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
                    table.PrimaryKey("PK_PhysicalParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalParams_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RotationalParams",
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
                    table.PrimaryKey("PK_RotationalParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RotationalParams_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StellarParams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AstroObjectId = table.Column<int>(type: "int", nullable: false),
                    SpectralClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Metallicity = table.Column<double>(type: "float", nullable: true),
                    Luminosity = table.Column<double>(type: "float", nullable: true),
                    Radiance = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StellarParams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StellarParams_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VSOP87D",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanetName = table.Column<string>(type: "varchar(10)", nullable: false),
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
                    table.PrimaryKey("PK_VSOP87D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VSOP87D_AstroObjects_AstroObjectId",
                        column: x => x.AstroObjectId,
                        principalTable: "AstroObjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtmoConstituents",
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
                    table.PrimaryKey("PK_AtmoConstituents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtmoConstituents_Atmospheres_AtmosphereId",
                        column: x => x.AtmosphereId,
                        principalTable: "Atmospheres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtmoConstituents_Molecules_MoleculeId",
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
                name: "IX_AtmoConstituents_AtmosphereId",
                table: "AtmoConstituents",
                column: "AtmosphereId");

            migrationBuilder.CreateIndex(
                name: "IX_AtmoConstituents_MoleculeId",
                table: "AtmoConstituents",
                column: "MoleculeId");

            migrationBuilder.CreateIndex(
                name: "IX_Atmospheres_AstroObjectId",
                table: "Atmospheres",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinorPlanetRecords_AstroObjectId",
                table: "MinorPlanetRecords",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MinorPlanetRecords_CoOrbitalObjectId",
                table: "MinorPlanetRecords",
                column: "CoOrbitalObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ObservationalParams_AstroObjectId",
                table: "ObservationalParams",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrbitalParams_AstroObjectId",
                table: "OrbitalParams",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalParams_AstroObjectId",
                table: "PhysicalParams",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RotationalParams_AstroObjectId",
                table: "RotationalParams",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StellarParams_AstroObjectId",
                table: "StellarParams",
                column: "AstroObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VSOP87D_AstroObjectId",
                table: "VSOP87D",
                column: "AstroObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AstroObjectAstroObjectGroup");

            migrationBuilder.DropTable(
                name: "AtmoConstituents");

            migrationBuilder.DropTable(
                name: "DeltaTEntries");

            migrationBuilder.DropTable(
                name: "EasterDates");

            migrationBuilder.DropTable(
                name: "LeapSeconds");

            migrationBuilder.DropTable(
                name: "LunarPhases");

            migrationBuilder.DropTable(
                name: "MinorPlanetRecords");

            migrationBuilder.DropTable(
                name: "ObservationalParams");

            migrationBuilder.DropTable(
                name: "OrbitalParams");

            migrationBuilder.DropTable(
                name: "PhysicalParams");

            migrationBuilder.DropTable(
                name: "RotationalParams");

            migrationBuilder.DropTable(
                name: "SeasonalMarkers");

            migrationBuilder.DropTable(
                name: "StellarParams");

            migrationBuilder.DropTable(
                name: "VSOP87D");

            migrationBuilder.DropTable(
                name: "AstroObjectGroups");

            migrationBuilder.DropTable(
                name: "Atmospheres");

            migrationBuilder.DropTable(
                name: "Molecules");

            migrationBuilder.DropTable(
                name: "AstroObjects");
        }
    }
}
