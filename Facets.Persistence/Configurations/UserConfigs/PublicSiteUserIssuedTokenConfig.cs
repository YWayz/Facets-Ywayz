using Facets.Core.Security.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.UserConfigs;

internal sealed class PublicSiteUserIssuedTokenConfig : IEntityTypeConfiguration<PublicSiteUserIssuedToken>
{
    public void Configure(EntityTypeBuilder<PublicSiteUserIssuedToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(p => p.UserIdentificationNumber).IsRequired().HasMaxLength(512);
        builder.Property(p => p.JTI).IsRequired().HasMaxLength(64);
    }
}
