using Facets.Infrastructure.FileStorage;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Facets.Api.Services;

/// <summary>
/// Applied to every string the API writes as JSON. Any blob URL pointing at this app's storage account is
/// replaced with a short-lived signed link, so DTOs and pass templates keep storing plain URLs while
/// clients always receive links that work against private containers.
/// </summary>
public sealed class SignedFileUrlJsonConverter : JsonConverter<string>
{
    private readonly IFileUrlSigner _signer;

    public SignedFileUrlJsonConverter(IFileUrlSigner signer)
    {
        _signer = signer;
    }

    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString()!;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(_signer.SignUrlsIn(value));
    }
}
