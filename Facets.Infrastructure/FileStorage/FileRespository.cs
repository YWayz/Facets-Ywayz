using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Facets.Core.Common.Dtos;
using Facets.Core.Common.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Facets.Infrastructure.FileStorage;

internal sealed class FileRespository : IFileRespository
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly FileStorageSettings _settings;
    private const string ContentType = "application/octet-stream";

    // Containers whose access level has already been checked in this process.
    private static readonly ConcurrentDictionary<string, bool> _preparedContainers = new(StringComparer.OrdinalIgnoreCase);

    public FileRespository(BlobServiceClient blobServiceClient, IOptions<FileStorageSettings> settings)
    {
        _blobServiceClient = blobServiceClient;
        _settings = settings.Value;
    }

    public async Task<ResponseResult> DeleteFile(string containerName, string blobName, CancellationToken token)
    {
        var blobContainerClient = await GetContainerAsync(containerName);

        var blob = blobContainerClient.GetBlobClient(blobName);

        var response = await blob.DeleteIfExistsAsync(cancellationToken: token);

        if (response.Value is false) return new ResponseResult(new BadRequestException("DeleteFailed", $"File ({blobName}) failed to delete"));

        return new();
    }

    public async Task<ResponseResult<Stream>> DownloadAsStream(string containerName, string blobName, CancellationToken token)
    {
        blobName = blobName.Replace(_blobServiceClient.Uri.OriginalString, string.Empty).Replace(containerName, string.Empty);

        var blobContainerClient = await GetContainerAsync(containerName);

        var blob = blobContainerClient.GetBlobClient(blobName);

        var response = await blob.DownloadContentAsync();

        if (response.GetRawResponse().IsError) return new(new OperationFailedException("File", "Failed to load document"));

        return new(response.Value.Content.ToStream());
    }

    public async Task<ResponseResult<FileUploadedResponse>> UploadFile(Stream fileStream, string fileName, string containerName, CancellationToken cancellationToken, string? folderPath = null, string? contentType = null)
    {
        var blobContainerClient = await GetContainerAsync(containerName);

        var extension = FileHelper.GetFileExtension(fileName);

        var uniqueFileName = $"{Guid.NewGuid()}{extension}";

        BlobClient blobClient = blobContainerClient.GetBlobClient($"{folderPath}/{uniqueFileName}");

        var blobContentInfo = await blobClient.UploadAsync(fileStream, new BlobHttpHeaders
        {
            ContentType = contentType ?? ContentType
        }, cancellationToken: cancellationToken);

        if (blobContentInfo.GetRawResponse().IsError) return new(new BadRequestException("FileUpload", $"File ({fileName}) failed to upload"));

        FileUploadedResponse file = new()
        {
            BlobName = blobClient.Name,
            FileName = fileName,
            UniqueName = uniqueFileName,
            URI = blobClient.Uri.ToString(),
        };

        return new(file);
    }

    private async Task<BlobContainerClient> GetContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        if (_preparedContainers.ContainsKey(containerName)) return containerClient;

        // Files here include NIC scans and photos, so containers are private and links are signed
        // (see BlobFileUrlSigner). Containers created public by earlier versions are switched to private.
        var accessType = _settings.PrivateContainers ? PublicAccessType.None : PublicAccessType.Blob;

        await containerClient.CreateIfNotExistsAsync(publicAccessType: accessType);

        if (_settings.PrivateContainers)
        {
            var properties = await containerClient.GetPropertiesAsync();

            if (properties.Value.PublicAccess is not null && properties.Value.PublicAccess != PublicAccessType.None)
            {
                await containerClient.SetAccessPolicyAsync(PublicAccessType.None);
            }
        }

        _preparedContainers[containerName] = true;

        return containerClient;
    }
}
