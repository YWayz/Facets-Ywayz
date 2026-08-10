using Facets.Core.Events.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Persistence.Configurations.EventConfigs;

internal sealed class PavilionSessionConfig : IEntityTypeConfiguration<PavilionSession>
{
    public void Configure(EntityTypeBuilder<PavilionSession> builder)
    {
        builder.HasQueryFilter(q => q.IsDeleted == false);

        builder.HasKey(e => e.Id);

        var navigation = builder.Metadata.FindNavigation(nameof(PavilionSession.VisitorPavilionSessionAttendanceSchedules));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.VisitorPavilionSessionAttendanceSchedules)
               .WithOne(s => s.PavilionSession)
               .HasForeignKey(f => f.PavilionSessionId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(s => s.Pavilion)
             .WithMany(s => s.PavilionSessions)
             .HasForeignKey(f => f.PavilionId)
             .OnDelete(DeleteBehavior.NoAction);
    }
}
