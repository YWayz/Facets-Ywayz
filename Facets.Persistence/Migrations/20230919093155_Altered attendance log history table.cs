using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Alteredattendanceloghistorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QRScannedByUserId",
                table: "VisitorAttendanceLog",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_RegistrationCounterId",
                table: "Invoice",
                column: "RegistrationCounterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_VisitorRegistrationCounter_RegistrationCounterId",
                table: "Invoice",
                column: "RegistrationCounterId",
                principalTable: "VisitorRegistrationCounter",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_VisitorRegistrationCounter_RegistrationCounterId",
                table: "Invoice");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_RegistrationCounterId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "QRScannedByUserId",
                table: "VisitorAttendanceLog");
        }
    }
}
