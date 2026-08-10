using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedverifiedatcolumntoOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "VerifiedAt",
                table: "OTPQueue",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OTPQueue_CreatedOn",
                table: "OTPQueue",
                column: "CreatedOn",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPQueue_CreatedOn",
                table: "OTPQueue");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "OTPQueue");
        }
    }
}
