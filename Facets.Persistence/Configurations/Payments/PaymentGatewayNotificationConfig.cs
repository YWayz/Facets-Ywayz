using Facets.Core.Payments.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.Payments;

internal sealed class PaymentGatewayNotificationConfig : IEntityTypeConfiguration<PaymentGatewayNotification>
{
    public void Configure(EntityTypeBuilder<PaymentGatewayNotification> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(a => a.TransactionId).HasMaxLength(256).IsRequired(false);
        builder.Property(a => a.PLRefNo).HasMaxLength(1024).IsRequired(false);
        builder.Property(a => a.StatusMessage).HasMaxLength(1024).IsRequired(false);
        builder.Property(a => a.AdditionalData).HasMaxLength(1500).IsRequired(false);
        builder.Property(a => a.DT).HasMaxLength(1024).IsRequired(false);
        builder.Property(a => a.InvoiceId).HasMaxLength(128).IsRequired(false);
    }
}
