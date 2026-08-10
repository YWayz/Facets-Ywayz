using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class uniqueconstrainsaddedvisitorpavilionattendancescheduletable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId_PavilionSessionId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                columns: new[] { "VisitorRegistrationId", "PavilionSessionId" },
                unique: true,
                filter: "Cancelled <> 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId_PavilionSessionId",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorRegistrationId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorRegistrationId");
        }
    }
}
