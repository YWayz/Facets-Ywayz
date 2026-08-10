using Facets.Core.Payments.Entities;
using Facets.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.Payments;

internal sealed class InvoiceLineItemConfig : IEntityTypeConfiguration<InvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Amount).DecimalPrecision();

        builder.HasIndex(x => new { x.ItemId, x.IsDeleted }).IsUnique().HasFilter("IsDeleted <> 1"); ;
    }
}
