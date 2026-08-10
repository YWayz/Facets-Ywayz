using Facets.Core.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Persistence.Configurations.EventConfigs;

internal sealed class PavilionConfig : IEntityTypeConfiguration<Pavilion>
{
    public void Configure(EntityTypeBuilder<Pavilion> builder)
    {
        builder.HasQueryFilter(q => q.IsDeleted == false);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
               .IsRequired()
               .HasMaxLength(StringLengths.FirstName);

        builder.HasOne(h => h.Event)
               .WithMany()
               .HasForeignKey(h => h.EventId);

        var navigation = builder.Metadata.FindNavigation(nameof(Pavilion.PavilionSessions));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(s => new { s.Name, s.EventId }).HasFilter($"{nameof(Pavilion.IsDeleted)} <> 1").IsUnique();
    }
}
