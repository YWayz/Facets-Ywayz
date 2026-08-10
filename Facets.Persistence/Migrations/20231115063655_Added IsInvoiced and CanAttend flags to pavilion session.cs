using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsInvoicedandCanAttendflagstopavilionsession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAttend",
                table: "VisitorPavilionSessionAttendanceSchedule",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInvoiced",
                table: "VisitorPavilionSessionAttendanceSchedule",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAttend",
                table: "VisitorPavilionSessionAttendanceSchedule");

            migrationBuilder.DropColumn(
                name: "IsInvoiced",
                table: "VisitorPavilionSessionAttendanceSchedule");
        }
    }
}
