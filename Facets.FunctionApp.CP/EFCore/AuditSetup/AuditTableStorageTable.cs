using Azure;
using Azure.Data.Tables;
using Facets.Persistence.AuditSetup;

namespace Facets.FunctionApp.CP.EFCore.AuditSetup;

public sealed class AuditTableStorageTable : ITableEntity
{
    public string? UserId { get; set; }
    public AuditType AuditType { get; set; }
    public string TableName { get; set; } = null!;
    public DateTimeOffset CreatedOn { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string PrimaryKey { get; set; } = null!;
    public Guid BatchId { get; set; }


    public string PartitionKey { get; set; } = null!;
    public string RowKey { get; set; } = null!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public static AuditTableStorageTable BuilData(Audit audit)
    {
        return new()
        {
            PartitionKey = audit.TableName,
            TableName = audit.TableName,
            AffectedColumns = audit.AffectedColumns,
            AuditType = audit.AuditType,
            BatchId = audit.BatchId,
            CreatedOn = audit.CreatedOn,
            OldValues = audit.OldValues,
            NewValues = audit.NewValues,
            PrimaryKey = audit.PrimaryKey,
            RowKey = Guid.NewGuid().ToString(),
            UserId = audit.UserId,
        };
    }
}
