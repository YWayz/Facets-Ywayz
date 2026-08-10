using Facets.Core.Security.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.UserConfigs;

internal sealed class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    private const string defaultAdminEmail = "admin@project.com";
    private const string adminUserName = "admin";

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasQueryFilter(w => w.IsDeleted == false);

        builder
            .HasOne(b => b.UserProfile)
            .WithOne(f => f.ApplicationUser)
            .HasForeignKey<UserProfile>(s => s.Id);

        var user = new ApplicationUser
        {
            Id = AppConstants.SuperAdmin.SuperUserId,
            UserName = adminUserName,
            Email = defaultAdminEmail,
            LockoutEnabled = false,
            PhoneNumber = "1234567890",
            CreatedOn = new DateTimeOffset(new DateTime(2022, 6, 30)),
            NormalizedEmail = defaultAdminEmail.ToUpper(),
            NormalizedUserName = adminUserName.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = "70428f75-0a6f-4d92-a2cd-ae4e0cdbd10f",
            ConcurrencyStamp = "70428f75-0a6f-4d92-a2cd-ae4e0cdbd10f",

            //)=hio(^0DWnsiSY9J9d4^pCG]ek){\*G
            PasswordHash = "AQAAAAIAB9AAAAAAEEBu3FWgRqgJWOiqjbX1yywHztopQIdUS0I23BsyaM8FxqlUionKPmAWRrD7obg/DQ=="
        };
        builder.HasData(user);
    }
}
