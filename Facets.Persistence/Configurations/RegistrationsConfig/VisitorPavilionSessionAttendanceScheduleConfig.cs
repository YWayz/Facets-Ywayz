using Facets.Core.Participants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.RegistrationsConfig;

internal sealed class VisitorPavilionSessionAttendanceScheduleConfig : IEntityTypeConfiguration<VisitorPavilionSessionAttendanceSchedule>
{
    public void Configure(EntityTypeBuilder<VisitorPavilionSessionAttendanceSchedule> builder)
    {
        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.HasKey(e => e.Id);

        builder.HasOne(s => s.VisitorRegistration)
               .WithMany(s => s.VisitorPavilionSessionAttendanceSchedules)
               .HasForeignKey(f => f.VisitorRegistrationId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(i => new { i.VisitorRegistrationId, i.PavilionSessionId })
               .HasFilter($"{nameof(VisitorPavilionSessionAttendanceSchedule.Cancelled)} <> 1")
               .IsUnique();
    }
}
