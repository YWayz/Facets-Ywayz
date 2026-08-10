using Facets.Core.Security.Dtos;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Security.Interfaces;

public interface IOTPStore
{
    Task<ResponseResult<string>> GenerateOTP(GenerateOTPDto model, CancellationToken cancellationToken);
    Task<ResponseResult<PublicUserAuthenticatedDto>> VerifyOTP(VerifyOTPDto model, CancellationToken cancellationToken);
}
