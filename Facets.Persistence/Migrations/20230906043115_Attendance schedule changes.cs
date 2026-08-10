using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Attendanceschedulechanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AttendanceScheduledOnsite",
                table: "VisitorAttendanceSchedule",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RegsitrationCounterId",
                table: "VisitorAttendanceSchedule",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceScheduledOnsite",
                table: "VisitorAttendanceSchedule");

            migrationBuilder.DropColumn(
                name: "RegsitrationCounterId",
                table: "VisitorAttendanceSchedule");
        }
    }
}
