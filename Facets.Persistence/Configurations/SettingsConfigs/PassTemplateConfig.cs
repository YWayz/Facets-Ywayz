using Facets.Core.Passes.Entities;
using Facets.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.SettingsConfigs;

internal sealed class PassTemplateConfig : IEntityTypeConfiguration<PassTemplate>
{
    public void Configure(EntityTypeBuilder<PassTemplate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.TemplateText).IsRequired();

        builder.Property(p => p.PreviewTemplateText).IsRequired();
        builder.Property(p => p.Height).DecimalPrecision();
        builder.Property(p => p.Width).DecimalPrecision();
        builder.Property(p => p.SizeType).IsRequired();
        builder.Property(p => p.PassType).IsRequired();

        builder.HasQueryFilter(p => p.IsDeleted == false);

        builder.HasOne(x => x.Event)
               .WithMany()
               .HasForeignKey(x => x.EventId);
    }
}
