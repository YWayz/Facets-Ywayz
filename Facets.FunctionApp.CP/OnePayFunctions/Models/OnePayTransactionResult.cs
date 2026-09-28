using System.Text.Json.Serialization;

namespace Facets.FunctionApp.CP.OnePayFunctions.Models;

public sealed class OnePayTransactionResult
{
    [JsonPropertyName("transaction_id")]
    public string? TransactionId { get; init; }

    [JsonPropertyName("pl_ref_no")]
    public string? PLRefNo { get; init; }

    [JsonPropertyName("status")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Status { get; init; }

    [JsonPropertyName("status_message")]
    public string? StatusMessage { get; init; }

    [JsonPropertyName("additional_data")]
    public string? AdditionalData { get; init; }
    
    [JsonPropertyName("dt")]
    public string? DT { get; init; }
}

