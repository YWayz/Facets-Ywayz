using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addedpublicsightuserissuedtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublicSiteUserIssuedToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserIdentificationNumber = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    JTI = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicSiteUserIssuedToken", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicSiteUserIssuedToken");
        }
    }
}
