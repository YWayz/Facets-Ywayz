using Facets.Core.Visitors.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.VisitorConfigs;

internal sealed class VisitorBlackListHistoryConfig : IEntityTypeConfiguration<VisitorBlackListHistory>
{
    public void Configure(EntityTypeBuilder<VisitorBlackListHistory> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Reason).IsRequired().HasMaxLength(AppConstants.StringLengths.Description);
        builder.Property(x => x.BlackListedUntil).IsRequired(false);
    }
}
