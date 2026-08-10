using Facets.Core.TeamMembers.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.TeamMemberConfigs;

internal sealed class TeamMemberEventConfig : IEntityTypeConfiguration<TeamMemberEvent>
{
    public void Configure(EntityTypeBuilder<TeamMemberEvent> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasQueryFilter(k => k.IsDeleted == false);

        builder.HasIndex(e => new { e.EventId, e.TeamMemberId, e.PassCategoryId }).IsUnique().HasFilter("IsDeleted <> 1");

        builder.HasOne(s => s.PassCategory)
               .WithMany()
               .HasForeignKey(f => f.PassCategoryId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
