using Facets.Core.Security.Dtos;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Interfaces;

internal interface IOTPService
{
    Task<ResponseResult<OTPInternalDto>> GenerateOTP(OTPType type, string identityNumber, string sendTo);
    Task<ResponseResult<OTPDto>> VerifyOTP(string identityNumber, string code, OTPType otpType);
}
