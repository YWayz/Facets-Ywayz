using Facets.Core.Participants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.RegistrationsConfig;

internal sealed class VisitorAttendanceLogConfig : IEntityTypeConfiguration<VisitorAttendanceLog>
{
    public void Configure(EntityTypeBuilder<VisitorAttendanceLog> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
