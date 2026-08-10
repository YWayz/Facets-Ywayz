using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.TeamMemberConfigs;

internal sealed class TeamMemberConfig : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(k => k.Id);
        builder.HasQueryFilter(k => k.IsDeleted == false);

        builder.Property(k => k.FirstName).IsRequired().HasMaxLength(AppConstants.StringLengths.FirstName);
        builder.Property(k => k.LastName).IsRequired().HasMaxLength(AppConstants.StringLengths.LastName);
        builder.Property(x => x.NICNumber).IsRequired(false).HasMaxLength(AppConstants.StringLengths.IdentityNumber).IsRequired(false);
        builder.HasIndex(x => x.NICNumber).IsUnique().HasFilter($"[{nameof(TeamMember.NICNumber)}] IS NOT NULL AND IsDeleted <> 1");

        builder.Property(k => k.PassportNumber).IsRequired(false).HasMaxLength(AppConstants.StringLengths.Description);
        builder.HasIndex(x => x.PassportNumber).IsUnique().HasFilter($"[{nameof(TeamMember.PassportNumber)}] IS NOT NULL AND IsDeleted <> 1");

        builder.Property(x => x.MobileNumber).HasMaxLength(AppConstants.StringLengths.PhoneNumber).IsRequired();

        builder.Property(k => k.CompanyName).IsRequired(false).HasMaxLength(AppConstants.StringLengths.Description);

        builder.OwnsOne(o => o.Address, c =>
        {
            c.WithOwner();

            c.Property(p => p.Address).HasMaxLength(AppConstants.StringLengths.Address);
        });

        builder.Navigation(n => n.Address).IsRequired(false);

        builder.HasOne(s => s.Country)
               .WithMany()
               .HasForeignKey(f => f.CountryId);
    }
}
