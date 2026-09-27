using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Facets.Infrastructure.FileStorage;

/// <summary>
/// Turns stored blob URLs into short-lived read-only signed links.
/// Stored URLs in the database stay plain; signing happens only when data leaves the API.
/// </summary>
public interface IFileUrlSigner
{
    /// <summary>
    /// Replaces every URL in <paramref name="text"/> that points at this app's storage account with a
    /// signed link. Text without such URLs is returned unchanged. Safe to call on any string,
    /// including HTML templates that embed image URLs.
    /// </summary>
    string SignUrlsIn(string text);
}

internal sealed class BlobFileUrlSigner : IFileUrlSigner
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly FileStorageSettings _settings;
    private readonly ILogger<BlobFileUrlSigner> _logger;
    private readonly string _accountBaseUrl;
    private readonly Regex _urlPattern;
    private bool _warnedCannotSign;

    public BlobFileUrlSigner(BlobServiceClient blobServiceClient, IOptions<FileStorageSettings> settings, ILogger<BlobFileUrlSigner> logger)
    {
        _blobServiceClient = blobServiceClient;
        _settings = settings.Value;
        _logger = logger;

        _accountBaseUrl = _blobServiceClient.Uri.ToString().TrimEnd('/');
        _urlPattern = new Regex(Regex.Escape(_accountBaseUrl) + @"/[^\s""'<>()\\]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public string SignUrlsIn(string text)
    {
        if (_settings.PrivateContainers is false || string.IsNullOrEmpty(text)) return text;

        if (text.Contains(_accountBaseUrl, StringComparison.OrdinalIgnoreCase) is false) return text;

        return _urlPattern.Replace(text, match => SignOne(match.Value));
    }

    private string SignOne(string url)
    {
        BlobUriBuilder parsed;

        try
        {
            parsed = new BlobUriBuilder(new Uri(url));
        }
        catch (UriFormatException)
        {
            return url;
        }

        if (string.IsNullOrEmpty(parsed.BlobContainerName) || string.IsNullOrEmpty(parsed.BlobName)) return url;

        if (_settings.PublicContainerSet.Contains(parsed.BlobContainerName)) return url;

        // Drop any old signature or query string so an already-signed URL (for example one that was saved
        // inside a pass template) is re-signed cleanly instead of getting a second signature appended.
        parsed.Sas = null;
        parsed.Query = string.Empty;

        var blobClient = _blobServiceClient.GetBlobContainerClient(parsed.BlobContainerName).GetBlobClient(parsed.BlobName);

        if (blobClient.CanGenerateSasUri is false)
        {
            if (_warnedCannotSign is false)
            {
                _warnedCannotSign = true;
                _logger.LogWarning("Storage client cannot sign URLs (no account key in the AzureStorage connection string); file links are returned unsigned and private containers will not load");
            }

            return parsed.ToUri().ToString();
        }

        var expiresOn = DateTimeOffset.UtcNow.AddHours(Math.Max(1, _settings.SignedUrlLifetimeHours));

        return blobClient.GenerateSasUri(BlobSasPermissions.Read, expiresOn).ToString();
    }
}
