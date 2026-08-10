using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedConfigsForThePavilion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PavilionSession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UpdatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DeletedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    AllowedVisitorCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    EventDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PavilionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PavilionSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PavilionSession_EventDate_EventDateId",
                        column: x => x.EventDateId,
                        principalTable: "EventDate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PavilionSession_Pavilion_PavilionId",
                        column: x => x.PavilionId,
                        principalTable: "Pavilion",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pavilion_Name_EventId",
                table: "Pavilion",
                columns: new[] { "Name", "EventId" },
                unique: true,
                filter: "IsDeleted <> 1");

            migrationBuilder.CreateIndex(
                name: "IX_PavilionSession_EventDateId",
                table: "PavilionSession",
                column: "EventDateId");

            migrationBuilder.CreateIndex(
                name: "IX_PavilionSession_IsDeleted",
                table: "PavilionSession",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PavilionSession_PavilionId",
                table: "PavilionSession",
                column: "PavilionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PavilionSession");

            migrationBuilder.DropIndex(
                name: "IX_Pavilion_Name_EventId",
                table: "Pavilion");
        }
    }
}
