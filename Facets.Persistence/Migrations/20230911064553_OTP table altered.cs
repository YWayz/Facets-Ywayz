using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OTPtablealtered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceEntitityId",
                table: "OTPQueue");

            migrationBuilder.DropColumn(
                name: "ReferenceEntitityName",
                table: "OTPQueue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceEntitityId",
                table: "OTPQueue",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceEntitityName",
                table: "OTPQueue",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
