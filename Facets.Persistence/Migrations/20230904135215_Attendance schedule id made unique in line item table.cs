using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Attendancescheduleidmadeuniqueinlineitemtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem",
                column: "VisitorAttendanceScheduleId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem",
                column: "VisitorAttendanceScheduleId");
        }
    }
}
