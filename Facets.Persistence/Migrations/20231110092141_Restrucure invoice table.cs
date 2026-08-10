using System;
using Facets.Core.Participants.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using static Facets.SharedKernal.AppEnums;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Restrucureinvoicetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceLineItem_VisitorAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "InvoiceLineItem");

            migrationBuilder.DropColumn(
                name: "VisitorAttendanceScheduleId",
                table: "InvoiceLineItem");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "InvoiceLineItem",
                newName: "ItemId");

            migrationBuilder.AddColumn<string>(
                name: "ItemReferenceEntityName",
                table: "InvoiceLineItem",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: typeof(VisitorAttendanceSchedule).FullName!);

            migrationBuilder.AddColumn<string>(
                name: "LineItemType",
                table: "InvoiceLineItem",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: nameof(InvoiceLineItemType.VisitorEventAttendance));

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_ItemId",
                table: "InvoiceLineItem",
                column: "ItemId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceLineItem_ItemId",
                table: "InvoiceLineItem");

            migrationBuilder.DropColumn(
                name: "ItemReferenceEntityName",
                table: "InvoiceLineItem");

            migrationBuilder.DropColumn(
                name: "LineItemType",
                table: "InvoiceLineItem");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "InvoiceLineItem",
                newName: "VisitorId");

            migrationBuilder.AddColumn<Guid>(
                name: "EventId",
                table: "InvoiceLineItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VisitorAttendanceScheduleId",
                table: "InvoiceLineItem",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceLineItem_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem",
                column: "VisitorAttendanceScheduleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceLineItem_VisitorAttendanceSchedule_VisitorAttendanceScheduleId",
                table: "InvoiceLineItem",
                column: "VisitorAttendanceScheduleId",
                principalTable: "VisitorAttendanceSchedule",
                principalColumn: "Id");
        }
    }
}
