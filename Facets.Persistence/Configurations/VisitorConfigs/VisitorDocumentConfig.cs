using Facets.Core.Visitors.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.VisitorConfigs;

internal sealed class VisitorDocumentConfig : IEntityTypeConfiguration<VisitorDocument>
{
    public void Configure(EntityTypeBuilder<VisitorDocument> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.Property(p => p.UniqueName).IsRequired(true).HasMaxLength(1024);
        builder.Property(p => p.DisplayName).IsRequired(true).HasMaxLength(1024);
        builder.Property(p => p.AttachmentURL).IsRequired(true).HasMaxLength(1024);
    }
}
