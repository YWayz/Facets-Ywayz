using Facets.Core.Visitors.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.VisitorConfigs;

internal sealed class VisitorActivityConfig : IEntityTypeConfiguration<VisitorActivity>
{
    public void Configure(EntityTypeBuilder<VisitorActivity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(s => s.Visitor)
               .WithMany()
               .HasForeignKey(x => x.VisitorId);

        builder.Property(p => p.Description).IsRequired().HasMaxLength(AppConstants.StringLengths.Description);
    }
}
