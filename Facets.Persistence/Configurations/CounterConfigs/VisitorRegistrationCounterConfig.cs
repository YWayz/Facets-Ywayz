using Facets.Core.Counters.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.CounterConfigs;

internal sealed class VisitorRegistrationCounterConfig : IEntityTypeConfiguration<VisitorRegistrationCounter>
{
    public void Configure(EntityTypeBuilder<VisitorRegistrationCounter> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(AppConstants.StringLengths.FirstName);
        builder.Property(x => x.Description).IsRequired(false).HasMaxLength(AppConstants.StringLengths.Description);

        builder.HasIndex(s => new { s.Name, s.EventId }).IsUnique().HasFilter("IsDeleted <> 1");

        builder.HasOne(x => x.Event)
               .WithMany()
               .HasForeignKey(x => x.EventId);

        builder.ToTable(name: nameof(VisitorRegistrationCounter),
                        table => table.IsTemporal());

        var navigation = builder.Metadata.FindNavigation(nameof(VisitorRegistrationCounter.UserAssignedRegistrationCounters));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
