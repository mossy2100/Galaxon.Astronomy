using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Galaxon.Astronomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedTableNameForIERSBulletinCs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeapSeconds");

            migrationBuilder.CreateTable(
                name: "IersBulletinCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BulletinNumber = table.Column<int>(type: "int", nullable: false),
                    BulletinUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateTimeParsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Value = table.Column<short>(type: "smallint", nullable: false),
                    LeapSecondDate = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IersBulletinCs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IersBulletinCs_BulletinNumber",
                table: "IersBulletinCs",
                column: "BulletinNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IersBulletinCs");

            migrationBuilder.CreateTable(
                name: "LeapSeconds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BulletinNumber = table.Column<int>(type: "int", nullable: false),
                    BulletinUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateTimeParsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeapSecondDate = table.Column<DateTime>(type: "date", nullable: true),
                    Value = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeapSeconds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeapSeconds_BulletinNumber",
                table: "LeapSeconds",
                column: "BulletinNumber",
                unique: true);
        }
    }
}
