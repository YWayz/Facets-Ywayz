using Facets.Core.Participants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.RegistrationsConfig;

internal sealed class VisitorAttendanceScheduleConfig : IEntityTypeConfiguration<VisitorAttendanceSchedule>
{
    public void Configure(EntityTypeBuilder<VisitorAttendanceSchedule> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(r => r.EventDate)
               .WithMany()
               .HasForeignKey(x => x.EventDateId)
               .OnDelete(DeleteBehavior.NoAction);

        var navigation = builder.Metadata.FindNavigation(nameof(VisitorAttendanceSchedule.VisitorAttendanceLog));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.VisitorAttendanceLog)
               .WithOne(s => s.VisitorAttendanceSchedule)
               .HasForeignKey(f => f.VisitorAttendanceScheduleId);

        builder.HasIndex(e => new { e.VisitorRegistrationId, e.EventDateId}).IsUnique().HasFilter("Cancelled <> 1");
    }
}
