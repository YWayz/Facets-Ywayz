using Facets.Core.Counters.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.CounterConfigs;

internal sealed class UserAssignedRegistrationCounterConfig : IEntityTypeConfiguration<UserAssignedRegistrationCounter>
{
    public void Configure(EntityTypeBuilder<UserAssignedRegistrationCounter> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(urc => urc.AssginedUser)
               .WithMany()
               .HasForeignKey(urc => urc.AssignedUserId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(urc => urc.VisitorRegistrationCounter)
               .WithMany(vrc => vrc.UserAssignedRegistrationCounters)
               .HasForeignKey(urc => urc.VisitorRegistrationCounterId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}
