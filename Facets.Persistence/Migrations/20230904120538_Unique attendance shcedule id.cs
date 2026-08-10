using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Uniqueattendanceshceduleid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorAttendanceSchedule_VisitorRegistrationId",
                table: "VisitorAttendanceSchedule");

            migrationBuilder.RenameColumn(
                name: "Paid",
                table: "VisitorAttendanceSchedule",
                newName: "CanAttend");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAttendanceSchedule_VisitorRegistrationId_EventDateId",
                table: "VisitorAttendanceSchedule",
                columns: new[] { "VisitorRegistrationId", "EventDateId" },
                unique: true,
                filter: "Cancelled <> 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorAttendanceSchedule_VisitorRegistrationId_EventDateId",
                table: "VisitorAttendanceSchedule");

            migrationBuilder.RenameColumn(
                name: "CanAttend",
                table: "VisitorAttendanceSchedule",
                newName: "Paid");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorAttendanceSchedule_VisitorRegistrationId",
                table: "VisitorAttendanceSchedule",
                column: "VisitorRegistrationId");
        }
    }
}
