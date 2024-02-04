using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Galaxon.Astronomy.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLeapSecondModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IersBulletinCDate",
                table: "LeapSeconds");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "LeapSeconds",
                newName: "BulletinNumber");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeapSecondDate",
                table: "LeapSeconds",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BulletinNumber",
                table: "LeapSeconds",
                newName: "Id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeapSecondDate",
                table: "LeapSeconds",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IersBulletinCDate",
                table: "LeapSeconds",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
