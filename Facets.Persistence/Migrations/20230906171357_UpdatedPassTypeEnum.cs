using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPassTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassTemplate_PassType_PassTypeId",
                table: "PassTemplate");

            migrationBuilder.DropIndex(
                name: "IX_PassTemplate_PassTypeId",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "PassTypeId",
                table: "PassTemplate");

            migrationBuilder.AddColumn<string>(
                name: "PassType",
                table: "PassTemplate",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PassType",
                table: "PassTemplate");

            migrationBuilder.AddColumn<Guid>(
                name: "PassTypeId",
                table: "PassTemplate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PassTemplate_PassTypeId",
                table: "PassTemplate",
                column: "PassTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PassTemplate_PassType_PassTypeId",
                table: "PassTemplate",
                column: "PassTypeId",
                principalTable: "PassType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
