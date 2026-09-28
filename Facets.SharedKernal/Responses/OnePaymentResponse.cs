using System.Text.Json.Serialization;

namespace Facets.SharedKernal.Responses;
public sealed class OnePaymentResponse<T>
{
    [JsonPropertyName("status")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Status { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }
}