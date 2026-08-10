using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.TeamMemberConfigs;

internal sealed class TeamMemberActivityConfig : IEntityTypeConfiguration<TeamMemberActivity>
{
    public void Configure(EntityTypeBuilder<TeamMemberActivity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(s => s.TeamMember)
               .WithMany()
               .HasForeignKey(x => x.TeamMemberId);

        builder.Property(p => p.Description).IsRequired().HasMaxLength(AppConstants.StringLengths.Description);
    }
}
