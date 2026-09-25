using System.Text.Json.Serialization;

namespace Facets.Infrastructure.OnePay.DTOs;

public sealed class OnePayPaymentRequestDto
{
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; init; }

    [JsonPropertyName("app_id")]
    public required string AppId { get; init; }

    [JsonPropertyName("reference")]
    public required string Reference { get; init; }

    [JsonPropertyName("customer_first_name")]
    public required string CustomerFirstName { get; init; }

    [JsonPropertyName("customer_last_name")]
    public required string CustomerLastName { get; init; }

    [JsonPropertyName("customer_phone_number")]
    public required string CustomerPhoneNumber { get; init; }

    [JsonPropertyName("customer_email")]
    public required string customerEmail { get; init; }

    [JsonPropertyName("transaction_redirect_url")]
    public required string TransactionRedirectUrl { get; init; }

    [JsonPropertyName("additionalData")]
    public required string AdditionalData { get; init; }

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;
}

