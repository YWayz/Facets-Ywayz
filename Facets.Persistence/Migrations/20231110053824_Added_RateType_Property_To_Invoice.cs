using Microsoft.EntityFrameworkCore.Migrations;
using static Facets.SharedKernal.AppEnums;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Added_RateType_Property_To_Invoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RateType",
                table: "Invoice",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: nameof(RateType.PerDayRate));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RateType",
                table: "Invoice");
        }
    }
}
