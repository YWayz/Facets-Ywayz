using Facets.Core.Payments.Entities;
using Facets.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.Payments;

internal sealed class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasQueryFilter(x => x.InvoiceCancelled == false);

        builder.HasOne(x => x.VisitorRegistration)
               .WithMany()
               .HasForeignKey(x => x.VisitorRegistrationId);

        var navigation = builder.Metadata.FindNavigation(nameof(Invoice.InvoiceLineItems));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.InvoiceLineItems)
               .WithOne(s => s.Invoice)
               .HasForeignKey(f => f.InvoiceId);

        builder.Property(x => x.TotalAmount).DecimalPrecision();

        builder.HasOne(s => s.VisitorRegistrationCounter)
               .WithMany()
               .HasForeignKey(f => f.RegistrationCounterId);

        navigation = builder.Metadata.FindNavigation(nameof(Invoice.Payments));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.Payments)
               .WithOne(s => s.Invoice)
               .HasForeignKey(f => f.InvoiceId);
    }
}
