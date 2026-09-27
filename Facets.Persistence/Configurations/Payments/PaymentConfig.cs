using Facets.Core.Payments.Entities;
using Facets.Persistence.Utilities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.Payments;

internal sealed class PaymentConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount).DecimalPrecision();

        builder.Property(x => x.LastFourDigitsOfCreditCard).IsRequired(false).HasMaxLength(AppConstants.StringLengths.IdentityNumber);
        builder.Property(x => x.CardPaymentReferenceNumber).IsRequired(false).HasMaxLength(AppConstants.StringLengths.IdentityNumber);

        builder.Property(x => x.InvoiceAmount).DecimalPrecision();

        // One online payment per OnePay transaction: stops a retried or concurrent webhook
        // notification from recording the same payment twice.
        builder.HasIndex(x => x.CardPaymentReferenceNumber)
               .IsUnique()
               .HasFilter("[IsOnlinePayment] = 1 AND [CardPaymentReferenceNumber] IS NOT NULL")
               .HasDatabaseName("IX_Payment_CardPaymentReferenceNumber");
    }
}
