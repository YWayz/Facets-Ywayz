using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedVisitorAttenceAndVisitorAttended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "VisitorAttended",
                table: "VisitorPavilionSessionAttendanceSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "VisitorAttendedAt",
                table: "VisitorPavilionSessionAttendanceSchedule",
                type: "datetimeoffset",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VisitorAttended",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.DropColumn(
                name: "VisitorAttendedAt",
                table: "VisitorPavilionSessionAttendanceSchedule");
        }
    }
}
