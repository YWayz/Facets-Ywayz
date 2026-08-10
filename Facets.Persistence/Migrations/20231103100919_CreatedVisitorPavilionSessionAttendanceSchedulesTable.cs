using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreatedVisitorPavilionSessionAttendanceSchedulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VisitorPavilionSessionAttendanceSchedule",
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
                    VisitorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PavilionSessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitorAttendanceScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancelledOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CancelledBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorPavilionSessionAttendanceSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisitorPavilionSessionAttendanceSchedule_PavilionSession_PavilionSessionId",
                        column: x => x.PavilionSessionId,
                        principalTable: "PavilionSession",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceSchedule_VisitorAttendanceScheduleId",
                        column: x => x.VisitorAttendanceScheduleId,
                        principalTable: "VisitorAttendanceSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitorPavilionSessionAttendanceSchedule_Visitor_VisitorId",
                        column: x => x.VisitorId,
                        principalTable: "Visitor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_IsDeleted",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_PavilionSessionId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "PavilionSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorAttendanceScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPavilionSessionAttendanceSchedule_VisitorId",
                table: "VisitorPavilionSessionAttendanceSchedule",
                column: "VisitorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VisitorPavilionSessionAttendanceSchedule");
        }
    }
}
