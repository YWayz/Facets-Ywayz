using Facets.Core.Passes.Entities;
using Facets.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.SettingsConfigs;

internal sealed class PassCategoryPavilionSettingsConfig : IEntityTypeConfiguration<PassCategoryPavilionSettings>
{
    public void Configure(EntityTypeBuilder<PassCategoryPavilionSettings> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(a => a.PassCategory)
               .WithMany(a => a.PassCategoryPavilionSettings)
               .HasForeignKey(a => a.PassCategoryId);

        builder.HasOne(a => a.Pavilion)
               .WithMany(a => a.PassCategoryPavilionSettings)
               .HasForeignKey(a => a.PavilionId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.Property(p => p.PavilionRate).DecimalPrecision();

        builder.ToTable(name: nameof(PassCategoryPavilionSettings),
                        table => table.IsTemporal());
    }
}
