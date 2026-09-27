using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Facets.Persistence.Migrations
{
    /// <summary>
    /// 1. OTPQueue.FailedAttempts: counts wrong codes so an OTP locks after
    ///    AppConstants.OTP.MaxFailedAttempts (stops brute-forcing the 6-digit code).
    /// 2. Unique index on online Payment.CardPaymentReferenceNumber (the OnePay transaction id),
    ///    so a retried or concurrent webhook notification cannot record the same payment twice.
    ///
    /// Written by hand (no .NET SDK was available when it was created), so it has no .Designer.cs;
    /// AppDbContextModelSnapshot was updated to match. The index fails to create if duplicate online
    /// payments already exist. Find them with:
    ///   SELECT CardPaymentReferenceNumber, COUNT(*) FROM Payment
    ///   WHERE IsOnlinePayment = 1 AND CardPaymentReferenceNumber IS NOT NULL
    ///   GROUP BY CardPaymentReferenceNumber HAVING COUNT(*) > 1;
    /// </summary>
    [DbContext(typeof(AppDbContext))]
    [Migration("20260928000000_PaymentAndOtpHardening")]
    public partial class PaymentAndOtpHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedAttempts",
                table: "OTPQueue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CardPaymentReferenceNumber",
                table: "Payment",
                column: "CardPaymentReferenceNumber",
                unique: true,
                filter: "[IsOnlinePayment] = 1 AND [CardPaymentReferenceNumber] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payment_CardPaymentReferenceNumber",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "FailedAttempts",
                table: "OTPQueue");
        }
    }
}
