using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.TeamMemberConfigs
{
    internal class TeamMemberDocumentConfig : IEntityTypeConfiguration<TeamMemberDocument>
    {
        public void Configure(EntityTypeBuilder<TeamMemberDocument> builder)
        {
            builder.HasKey(k => k.Id);
            builder.HasQueryFilter(k => k.IsDeleted == false);


            builder.Property(k => k.AttachmentURL).IsRequired().HasMaxLength(AppConstants.StringLengths.Description);
        }
    }
}
