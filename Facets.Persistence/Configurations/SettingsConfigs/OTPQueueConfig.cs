using Facets.Core.Security.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.SettingsConfigs;

internal sealed class OTPQueueConfig : IEntityTypeConfiguration<OTPQueue>
{
    public void Configure(EntityTypeBuilder<OTPQueue> builder)
    {
        builder.HasKey(k => k.Id);
        builder.Property(k => k.Code).IsRequired().HasMaxLength(AppConstants.OTP.Length);
        builder.Property(k => k.IdentityNumber).IsRequired().HasMaxLength(AppConstants.StringLengths.IdentityNumber);
        builder.Property(k => k.SentTo).IsRequired().HasMaxLength(AppConstants.StringLengths.Email);

        builder.HasIndex(p => p.CreatedOn).IsDescending(true);
    }
}
