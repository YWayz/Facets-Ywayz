using System.Text.Json;
using System.Text.Json.Serialization;

namespace Facets.Infrastructure.OnePay.DTOs;

/// <summary>
/// Body sent to POST /v3/transaction/status/.
/// </summary>
internal sealed class OnePayTransactionStatusRequestDto
{
    [JsonPropertyName("app_id")]
    public required string AppId { get; init; }

    [JsonPropertyName("onepay_transaction_id")]
    public required string OnePayTransactionId { get; init; }
}

/// <summary>
/// The "data" object returned by POST /v3/transaction/status/.
/// This is the authoritative answer from OnePay on whether a transaction was paid,
/// and is used to verify webhook notifications (which carry no signature).
/// </summary>
public sealed class OnePayTransactionStatusDto
{
    [JsonPropertyName("status")]
    [JsonConverter(typeof(LenientBoolJsonConverter))]
    public bool Paid { get; init; }

    [JsonPropertyName("ipg_transaction_id")]
    public string? IPGTransactionId { get; init; }

    [JsonPropertyName("amount")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("paid_on")]
    public string? PaidOn { get; init; }
}

/// <summary>
/// Accepts true/false, "true"/"false", 1/0 and "1"/"0". OnePay's documentation shows the paid flag
/// as a boolean, but their other fields have shifted type between API versions; a wrong type here
/// would otherwise throw and block every payment from being recorded.
/// </summary>
public sealed class LenientBoolJsonConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True: return true;
            case JsonTokenType.False: return false;
            case JsonTokenType.Number: return reader.TryGetInt64(out long n) ? n != 0 : reader.GetDouble() != 0;
            case JsonTokenType.String:
                string? text = reader.GetString();
                if (bool.TryParse(text, out bool b)) return b;
                if (long.TryParse(text, out long l)) return l != 0;
                return false;
            case JsonTokenType.Null: return false;
            default: throw new JsonException($"Cannot convert {reader.TokenType} to bool");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) => writer.WriteBooleanValue(value);
}
