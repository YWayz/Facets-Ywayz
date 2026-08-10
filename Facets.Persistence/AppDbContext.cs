using Facets.Core.Security.Entities;
using Facets.Persistence.AuditSetup;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser, Role, Guid, UserClaim, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IDomainEventDispatcher? _dispatcher;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public AppDbContext(DbContextOptions<AppDbContext> options, ILoggedInUserService loggedInUserService, IDomainEventDispatcher? dispatcher) : base(options)
    {
        _loggedInUserService = loggedInUserService;
        _dispatcher = dispatcher;
        SavingChanges += ModifyAuditInformation;
    }

    private void ModifyAuditInformation(object? sender, SavingChangesEventArgs e)
    {
        ChangeTracker.DetectChanges();

        List<Audit> auditEntries = new();
        var auditTable = Set<Audit>();

        foreach (var entry in ChangeTracker.Entries())
        {
            var entity = entry.Entity;

            if (entity is ICreatedAudit createdEntry && entry.State == EntityState.Added)
            {
                createdEntry.CreatedOn = DateTimeOffset.UtcNow;
                createdEntry.CreatedBy = _loggedInUserService.UserId;
            }

            else if (entity is IDeletedAudit deletedEntry and { IsDeleted: true })
            {
                deletedEntry.DeletedOn = DateTimeOffset.UtcNow;
                deletedEntry.DeletedBy = _loggedInUserService.UserId;
            }

            else if (entity is IUpdatedAudit updatedEntry && entry.State == EntityState.Modified)
            {
                updatedEntry.UpdatedOn = DateTimeOffset.UtcNow;
                updatedEntry.UpdatedBy = _loggedInUserService.UserId;
            }

            var audit = OnBeforeSaveChanges(entry, ContextId.InstanceId);

            if (audit is not null) auditEntries.Add(audit);
        }

        auditTable.AddRange(auditEntries);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await DispatchPrePersistantDomainEvents();

        int result = await base.SaveChangesAsync(cancellationToken);

        await DispatchPostPersistantDomainEvents();

        return result;
    }

    private async Task DispatchPostPersistantDomainEvents()
    {
        if (_dispatcher == null) return;

        var entitiesWithPostEvents = ChangeTracker.Entries<EntityBase>()
           .Select(e => e.Entity)
           .Where(e => e.DomainEvents.Any(a => a.IsPrePersistantDomainEvent == false))
           .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithPostEvents, isPrePersistantDomainEvent: false);
    }

    private async Task DispatchPrePersistantDomainEvents()
    {
        if (_dispatcher == null) return;

        var entitiesWithPreEvents = ChangeTracker.Entries<EntityBase>()
          .Select(e => e.Entity)
          .Where(e => e.DomainEvents.Any(w => w.IsPrePersistantDomainEvent))
          .ToArray();

        await _dispatcher.DispatchAndClearEvents(entitiesWithPreEvents, isPrePersistantDomainEvent: true);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        builder.ApplyCommanConfigurations();

        ApplyGlobalFilters(builder);

        base.OnModelCreating(builder);
    }

    private void ApplyGlobalFilters(ModelBuilder builder)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<AppEnums.VisitorPassCategoryType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.TeamMemberActivityType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.VisitorIdentityType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.VisitorActivityType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.InvoiceLineItemType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.TemplateSizeType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.TeamMemberStatus>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.PassCategoryType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.OnSitePayingMode>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.PavilionStatus>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.AttachmentType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.PaymentMethod>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.VisitorStatus>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.PaymentStatus>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.DiscountType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.CounterType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.EventStatus>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.RateType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.PassType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.OTPType>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.Title>().HaveConversion<string>().HaveMaxLength(64);
        configurationBuilder.Properties<AppEnums.MemberBlacklistStatus>().HaveConversion<string>().HaveMaxLength(64);
    }

    private Audit? OnBeforeSaveChanges(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, Guid batchId)
    {
        if (entry.Entity is INoAudit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) return default;

        var auditEntry = new AuditEntry(entry, batchId)
        {
            TableName = entry.Entity.GetType().Name,
            UserId = _loggedInUserService.UserId
        };

        foreach (var property in entry.Properties)
        {
            string propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.PrimaryKey = property.CurrentValue!.ToString()!;
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.AuditType = AuditType.Create;
                    auditEntry.NewValues[propertyName] = property.CurrentValue!;
                    break;

                case EntityState.Deleted:
                    auditEntry.AuditType = AuditType.Delete;
                    auditEntry.OldValues[propertyName] = property.OriginalValue!;
                    break;

                case EntityState.Modified:
                    if (property.IsModified)
                    {
                        auditEntry.ChangedColumns.Add(propertyName);
                        auditEntry.AuditType = AuditType.Update;
                        auditEntry.OldValues[propertyName] = property.OriginalValue!;
                        auditEntry.NewValues[propertyName] = property.CurrentValue!;
                    }
                    break;
            }
        }

        return auditEntry.ToAudit();
    }
}
