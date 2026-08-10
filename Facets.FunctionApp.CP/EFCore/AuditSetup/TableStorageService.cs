using Azure.Data.Tables;

namespace Facets.FunctionApp.CP.EFCore.AuditSetup;

internal class TableStorageService : ITableStorageService
{
    private readonly TableServiceClient _tableServiceClient;

    public TableStorageService(TableServiceClient tableServiceClient)
    {
        _tableServiceClient = tableServiceClient;
    }

    public async Task BulkInsert(string tableName, string partionKey, List<AuditTableStorageTable> audit)
    {
        var client = _tableServiceClient.GetTableClient(tableName);

        List<TableTransactionAction> addEntitiesBatch = new List<TableTransactionAction>();

        addEntitiesBatch.AddRange(audit.Select(e => new TableTransactionAction(TableTransactionActionType.Add, e)));

        await client.SubmitTransactionAsync(addEntitiesBatch);

    }

    public async Task CreateTableIfNotExistsAsync(string tableName)
    {
        var client = _tableServiceClient.GetTableClient(tableName);

        await client.CreateIfNotExistsAsync();
    }
}
