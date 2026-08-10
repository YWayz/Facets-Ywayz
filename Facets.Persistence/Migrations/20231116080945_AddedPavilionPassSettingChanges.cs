using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPavilionPassSettingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassCategoryPavilionSettings_Pavilion_PassCategoryId",
                table: "PassCategoryPavilionSettings");

            migrationBuilder.CreateIndex(
                name: "IX_PassCategoryPavilionSettings_PavilionId",
                table: "PassCategoryPavilionSettings",
                column: "PavilionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PassCategoryPavilionSettings_Pavilion_PavilionId",
                table: "PassCategoryPavilionSettings",
                column: "PavilionId",
                principalTable: "Pavilion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassCategoryPavilionSettings_Pavilion_PavilionId",
                table: "PassCategoryPavilionSettings");

            migrationBuilder.DropIndex(
                name: "IX_PassCategoryPavilionSettings_PavilionId",
                table: "PassCategoryPavilionSettings");

            migrationBuilder.AddForeignKey(
                name: "FK_PassCategoryPavilionSettings_Pavilion_PassCategoryId",
                table: "PassCategoryPavilionSettings",
                column: "PassCategoryId",
                principalTable: "Pavilion",
                principalColumn: "Id");
        }
    }
}
