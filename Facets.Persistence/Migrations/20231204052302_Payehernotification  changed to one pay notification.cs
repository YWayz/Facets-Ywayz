using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Payehernotificationchangedtoonepaynotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardExpiry",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "CardHolderName",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "CardNo",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "Custom1",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "Custom2",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "MD5sSig",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "MerchantId",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "PaymentGatewayNotification");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                table: "PaymentGatewayNotification");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "PaymentGatewayNotification",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "PayHereCurrency",
                table: "PaymentGatewayNotification",
                newName: "PLRefNo");

            migrationBuilder.RenameColumn(
                name: "PayHereAmount",
                table: "PaymentGatewayNotification",
                newName: "AdditionalData");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PaymentGatewayNotification",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentGatewayNotification");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "PaymentGatewayNotification",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "PLRefNo",
                table: "PaymentGatewayNotification",
                newName: "PayHereCurrency");

            migrationBuilder.RenameColumn(
                name: "AdditionalData",
                table: "PaymentGatewayNotification",
                newName: "PayHereAmount");

            migrationBuilder.AddColumn<string>(
                name: "CardExpiry",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardHolderName",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNo",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Custom1",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Custom2",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceId",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MD5sSig",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchantId",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "PaymentGatewayNotification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusCode",
                table: "PaymentGatewayNotification",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}
