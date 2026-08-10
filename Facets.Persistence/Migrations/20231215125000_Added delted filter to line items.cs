using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addeddeltedfiltertolineitems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_ItemId",
                table: "InvoiceLineItem");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_ItemId_IsDeleted",
                table: "InvoiceLineItem",
                columns: new[] { "ItemId", "IsDeleted" },
                unique: true,
                filter: "IsDeleted <> 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_ItemId_IsDeleted",
                table: "InvoiceLineItem");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_ItemId",
                table: "InvoiceLineItem",
                column: "ItemId",
                unique: true);
        }
    }
}
