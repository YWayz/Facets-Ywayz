using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addedvisitorregistrationidandremovevisitoridfromvisitorpavilionattendancescheduletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_Visitor_VisitorId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                newName: "VisitorRegistrationId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                newName: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_VisitorRegistration_VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorRegistrationId",
                principalTable: "VisitorRegistration",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_VisitorRegistration_VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.RenameColumn(
                name: "VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                newName: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitorPavilionSessionAttendanceSchedule_Visitor_VisitorId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorId",
                principalTable: "Visitor",
                principalColumn: "Id");
        }
    }
}
