using Facets.Core.Passes.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Configurations.SettingsConfigs;

internal sealed class PassCategoryConfig : IEntityTypeConfiguration<PassCategory>
{
    public void Configure(EntityTypeBuilder<PassCategory> builder)
    {
        builder.HasKey(k => k.Id);
        builder.HasQueryFilter(k => k.IsDeleted == false);

        builder.Property(k => k.Name).IsRequired().HasMaxLength(AppConstants.StringLengths.Description);
        builder.HasIndex(k => new { k.Name, k.EventId }).IsUnique().HasFilter("IsDeleted <> 1");

        builder.Property(k => k.Description).IsRequired(false).HasMaxLength(AppConstants.StringLengths.Description);

        builder.HasOne(x => x.PassType)
               .WithMany()
               .HasForeignKey(x => x.PassTypeId);

        var navigation = builder.Metadata.FindNavigation(nameof(PassCategory.PassCategorySettings));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.ToTable(name: nameof(PassCategory),
                        table => table.IsTemporal());

        builder.HasQueryFilter(p => p.IsDeleted == false);

        builder.HasOne(x => x.Event)
               .WithMany()
               .HasForeignKey(x => x.EventId);

        builder.Property(p => p.VisitorPassCategoryType).HasDefaultValue(VisitorPassCategoryType.PerDayPass);
        builder.Property(k => k.Color).IsRequired().HasMaxLength(AppConstants.StringLengths.Color).HasDefaultValue("#000000");
    }
}
