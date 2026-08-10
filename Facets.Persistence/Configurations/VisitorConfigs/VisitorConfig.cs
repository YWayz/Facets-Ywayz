using Facets.Core.Visitors.Entities;
using Facets.SharedKernal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Facets.Persistence.Configurations.VisitorConfigs;

internal sealed class VisitorConfig : IEntityTypeConfiguration<Visitor>
{
    public void Configure(EntityTypeBuilder<Visitor> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasQueryFilter(x => x.IsDeleted == false);

        builder.Property(x => x.NICNumber).HasMaxLength(AppConstants.StringLengths.IdentityNumber).IsRequired(false);
        builder.HasIndex(x => x.NICNumber).IsUnique().HasFilter($"[{nameof(Visitor.NICNumber)}] IS NOT NULL AND IsDeleted <> 1");

        builder.Property(x => x.PassportNumber).HasMaxLength(AppConstants.StringLengths.IdentityNumber).IsRequired(false);
        builder.HasIndex(x => x.PassportNumber).IsUnique().HasFilter($"[{nameof(Visitor.PassportNumber)}] IS NOT NULL AND IsDeleted <> 1");

        builder.HasOne(s => s.Country)
               .WithMany()
               .HasForeignKey(s => s.CountryId);

        builder.Property(x => x.FirstName).HasMaxLength(AppConstants.StringLengths.FirstName).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(AppConstants.StringLengths.LastName).IsRequired();
        builder.Property(x => x.MobileNumber).HasMaxLength(AppConstants.StringLengths.PhoneNumber).IsRequired();
        builder.Property(x => x.CompanyName).HasMaxLength(AppConstants.StringLengths.Description).IsRequired(false);
        builder.Property(x => x.Email).HasMaxLength(AppConstants.StringLengths.Email).IsRequired(false);

        builder.OwnsOne(o => o.Address, c =>
        {
            c.WithOwner();

            c.Property(p => p.Address).HasMaxLength(AppConstants.StringLengths.Address);
        });

        builder.Navigation(n => n.Address).IsRequired(false);

        builder.ToTable(nameof(Visitor),
                        tableBuilder =>
                        {
                            tableBuilder.IsTemporal();
                            tableBuilder.Property<DateTime>("PeriodStart").HasColumnName("PeriodStart");
                            tableBuilder.Property<DateTime>("PeriodEnd").HasColumnName("PeriodEnd");
                        })
                        .OwnsOne(visitor => visitor.Address,
                                 ownedBuilder => ownedBuilder.ToTable(
                                 nameof(Visitor),
                                 tableBuilder =>
                                 {
                                     tableBuilder.IsTemporal();
                                     tableBuilder.Property<DateTime>("PeriodStart").HasColumnName("PeriodStart");
                                     tableBuilder.Property<DateTime>("PeriodEnd").HasColumnName("PeriodEnd");
                                 }));


        var navigation = builder.Metadata.FindNavigation(nameof(Visitor.Documents));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.Documents)
               .WithOne(s => s.Visitor)
               .HasForeignKey(f => f.VisitorId);

        navigation = builder.Metadata.FindNavigation(nameof(Visitor.VisitorBlackListHistories));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(s => s.VisitorBlackListHistories)
               .WithOne(s => s.Visitor)
               .HasForeignKey(f => f.VisitorId);
    }
}
