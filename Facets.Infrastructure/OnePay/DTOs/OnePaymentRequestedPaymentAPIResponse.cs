using System.Text.Json.Serialization;

internal sealed class OnePaymentRequestedPaymentAPIResponse
{
    [JsonPropertyName("ipg_transaction_id")]
    public string? IPGTransactionId { get; init; }

    [JsonPropertyName("amount")]
    public Amount? Amount { get; init; }

    [JsonPropertyName("gateway")]
    public Gateway? Gateway { get; init; }
}

internal sealed class Amount
{
    [JsonPropertyName("gross_amount")]
    public decimal GrossAmount { get; init; }

    [JsonPropertyName("discount")]
    public decimal Discount { get; init; }

    [JsonPropertyName("handling_fee")]
    public decimal HandlingFee { get; init; }

    [JsonPropertyName("net_amount")]
    public decimal NetAmount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
}

public sealed class Gateway
{
    [JsonPropertyName("redirect_url")]
    public string? RedirectURL { get; init; }
}
