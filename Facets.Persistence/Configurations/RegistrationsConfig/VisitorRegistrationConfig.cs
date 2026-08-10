using Facets.Core.Participants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.RegistrationsConfig;

internal sealed class VisitorRegistrationConfig : IEntityTypeConfiguration<VisitorRegistration>
{
    public void Configure(EntityTypeBuilder<VisitorRegistration> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.EventId, e.VisitorId }).IsUnique().HasFilter("RegistrationCancelled <> 1");

        builder.HasOne(r => r.Event)
               .WithMany()
               .HasForeignKey(x => x.EventId);

        builder.HasOne(r => r.Visitor)
               .WithMany()
               .HasForeignKey(x => x.VisitorId);

        var navigation = builder.Metadata.FindNavigation(nameof(VisitorRegistration.VisitorAttendanceSchedules));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.VisitorAttendanceSchedules)
               .WithOne(s => s.VisitorRegistration)
               .HasForeignKey(f => f.VisitorRegistrationId);

        builder.HasOne(s => s.PassCategory)
               .WithMany()
               .HasForeignKey(f => f.PassCategoryId)
               .OnDelete(DeleteBehavior.NoAction);

        navigation = builder.Metadata.FindNavigation(nameof(VisitorRegistration.VisitorPavilionSessionAttendanceSchedules));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
