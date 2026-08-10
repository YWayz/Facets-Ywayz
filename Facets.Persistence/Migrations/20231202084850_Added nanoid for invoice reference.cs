using Microsoft.EntityFrameworkCore.Migrations;
using NanoidDotNet;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addednanoidforinvoicereference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reference",
                table: "Invoice",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: Nanoid.Generate(size: 20));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reference",
                table: "Invoice");
        }
    }
}
