using Facets.Core.Common.Dtos;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Common.Interfaces;

public interface IFileRespository
{
    Task<ResponseResult<FileUploadedResponse>> UploadFile(Stream fileStream, string documentName, string containerName, CancellationToken cancellationToken, string? folderPath = null, string? contentType = null);

    Task<ResponseResult<Stream>> DownloadAsStream(string containerName, string blobName, CancellationToken token);

    Task<ResponseResult> DeleteFile(string containerName, string blobName, CancellationToken cancellationToken);

}
