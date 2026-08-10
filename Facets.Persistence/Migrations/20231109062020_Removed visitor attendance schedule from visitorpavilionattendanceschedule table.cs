using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Removedvisitorattendanceschedulefromvisitorpavilionattendancescheduletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.DropIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.DropColumn(
                name: "VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorAttendanceScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorAttendanceScheduleId",
                principalTable: "VisitorAttendanceSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
