using Facets.Core.Common.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Configurations.SettingsConfigs;

internal sealed class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(64);
        builder.Property(p => p.Code).IsRequired().HasMaxLength(4);
    }
}
