using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Filteroutdeletedeventwhencheckingforuniquename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Event_Name",
                table: "Event");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Name",
                table: "Event",
                column: "Name",
                unique: true,
                filter: "IsDeleted <> 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Event_Name",
                table: "Event");

            migrationBuilder.CreateIndex(
                name: "IX_Event_Name",
                table: "Event",
                column: "Name",
                unique: true);
        }
    }
}
