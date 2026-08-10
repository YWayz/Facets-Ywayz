using Facets.FunctionApp.CP.EFCore.AuditSetup;
using Facets.Persistence;
using Facets.Persistence.AuditSetup;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;

namespace Facets.FunctionApp.CP.ArchiveAppAuditLogs;

public sealed class ArchiveAppAuditLogTimerTrigger
{
    private readonly AppDbContext _dbContext;
    private readonly ITableStorageService _tableStorageService;

    public ArchiveAppAuditLogTimerTrigger(AppDbContext dbContext, ITableStorageService tableStorageService)
    {
        _dbContext = dbContext;
        _tableStorageService = tableStorageService;
    }

    [Function(nameof(ArchiveAppAuditLogTimerTrigger))]
    public async Task ArchiveAppAuditLogTrigger([TimerTrigger("%ArchiveAppAuditLogCron%")] TimerInfo myTimer)
    {
        var currentDate = DateTimeOffset.UtcNow.Date;

        var auditTable = _dbContext.Set<Audit>();

        await _tableStorageService.CreateTableIfNotExistsAsync(FuncAppConstants.AuditTableName);

        var query = auditTable.Where(w => w.CreatedOn < currentDate);

        int pageSize = 100;
        bool hasMoreRecords = true;

        for (int skip = 1; hasMoreRecords is true; skip++)
        {
            var recordsToSync = await query.Skip(0)
                                           .Take(pageSize)
                                           .ToListAsync();

            hasMoreRecords = recordsToSync.Any();

            if (hasMoreRecords is false) break;

            var groupedAuditData = recordsToSync.GroupBy(b => b.TableName).ToList();

            foreach (var groupedAuditDatem in groupedAuditData)
            {
                List<AuditTableStorageTable> auditStorageTableRecords = new();

                foreach (var auditEntry in groupedAuditDatem.ToList()) auditStorageTableRecords.Add(AuditTableStorageTable.BuilData(auditEntry));

                await _tableStorageService.BulkInsert(FuncAppConstants.AuditTableName, groupedAuditDatem.Key, auditStorageTableRecords);

            }
            auditTable.RemoveRange(recordsToSync);
            await _dbContext.SaveChangesAsync();
        }
    }
}
