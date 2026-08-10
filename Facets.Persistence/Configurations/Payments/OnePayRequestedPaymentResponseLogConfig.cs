using Facets.Core.Payments.Entities;
using Facets.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.Payments;
internal sealed class OnePayRequestedPaymentResponseLogConfig : IEntityTypeConfiguration<OnePayRequestedPaymentResponseLog>
{
    public void Configure(EntityTypeBuilder<OnePayRequestedPaymentResponseLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.IPGTransactionId).IsRequired(false).HasMaxLength(256);
        builder.Property(x => x.GrossAmount).DecimalPrecision();
        builder.Property(x => x.HandlingFee).DecimalPrecision();
        builder.Property(x => x.NetAmount).DecimalPrecision();
        builder.Property(x => x.Currency).IsRequired(false).HasMaxLength(8);
        builder.Property(x => x.InvoiceReferenceNumber).HasMaxLength(32);
    }
}
