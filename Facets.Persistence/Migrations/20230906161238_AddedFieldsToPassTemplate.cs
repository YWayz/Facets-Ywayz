using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldsToPassTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HTMLText",
                table: "PassTemplate",
                newName: "TemplateText");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "PassTemplate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "PassTemplate",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PassTypeId",
                table: "PassTemplate",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PreviewTemplateText",
                table: "PassTemplate",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "PassTemplate",
                type: "decimal(18,6)",
                precision: 18,
                scale: 6,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_PassTemplate_EventId",
                table: "PassTemplate",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_PassTemplate_PassTypeId",
                table: "PassTemplate",
                column: "PassTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PassTemplate_Event_EventId",
                table: "PassTemplate",
                column: "EventId",
                principalTable: "Event",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PassTemplate_PassType_PassTypeId",
                table: "PassTemplate",
                column: "PassTypeId",
                principalTable: "PassType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PassTemplate_Event_EventId",
                table: "PassTemplate");

            migrationBuilder.DropForeignKey(
                name: "FK_PassTemplate_PassType_PassTypeId",
                table: "PassTemplate");

            migrationBuilder.DropIndex(
                name: "IX_PassTemplate_EventId",
                table: "PassTemplate");

            migrationBuilder.DropIndex(
                name: "IX_PassTemplate_PassTypeId",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "PassTypeId",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "PreviewTemplateText",
                table: "PassTemplate");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "PassTemplate");

            migrationBuilder.RenameColumn(
                name: "TemplateText",
                table: "PassTemplate",
                newName: "HTMLText");
        }
    }
}
