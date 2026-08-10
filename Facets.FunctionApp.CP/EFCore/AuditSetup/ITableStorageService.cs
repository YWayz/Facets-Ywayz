namespace Facets.FunctionApp.CP.EFCore.AuditSetup;

public interface ITableStorageService
{
    Task CreateTableIfNotExistsAsync(string tableName);
    Task BulkInsert(string tableName, string partionKey, List<AuditTableStorageTable> audit);
}
