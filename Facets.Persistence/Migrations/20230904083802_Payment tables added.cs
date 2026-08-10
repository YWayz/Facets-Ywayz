using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Paymenttablesadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VisitorRegistration_EventId_VisitorId",
                table: "VisitorRegistration");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaidOn",
                table: "Invoice",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastFourDigitsOfCreditCard = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CardPaymentReferenceNumber = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentDetail_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentDetail_Payment_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VisitorRegistration_EventId_VisitorId",
                table: "VisitorRegistration",
                columns: new[] { "EventId", "VisitorId" },
                unique: true,
                filter: "RegistrationCancelled <> 1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetail_InvoiceId",
                table: "PaymentDetail",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDetail_PaymentId",
                table: "PaymentDetail",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentDetail");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_VisitorRegistration_EventId_VisitorId",
                table: "VisitorRegistration");

            migrationBuilder.DropColumn(
                name: "PaidOn",
                table: "Invoice");

            migrationBuilder.CreateIndex(
                name: "IX_VisitorRegistration_EventId_VisitorId",
                table: "VisitorRegistration",
                columns: new[] { "EventId", "VisitorId" },
                unique: true,
                filter: "IsDeleted <> 1");
        }
    }
}
