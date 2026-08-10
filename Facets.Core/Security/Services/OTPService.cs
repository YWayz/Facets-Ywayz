using Facets.Core.Security.Dtos;
using Facets.Core.Security.Entities;
using Facets.Core.Security.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using NanoidDotNet;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Services;

internal sealed class OTPService : IOTPService
{
    private readonly IOTPRepository _otpRepository;

    public OTPService(IOTPRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public async Task<ResponseResult<OTPInternalDto>> GenerateOTP(OTPType type, string identityNumber, string sendTo)
    {
        var code = await Nanoid.GenerateAsync(AppConstants.OTP.Characters, AppConstants.OTP.Length);

        OTPQueue otp = new(code, type, identityNumber, sendTo);

        _otpRepository.Add(otp);

        return new(new OTPInternalDto(otp.Id, otp.Type, otp.IdentityNumber, otp.SentTo, otp.Code));
    }

    public async Task<ResponseResult<OTPDto>> VerifyOTP(string identityNumber, string code, OTPType otpType)
    {
        var otp = await _otpRepository.GetOTPToVerify(identityNumber);

        if (otp is null || otp.Code != code) return new(new OperationFailedException("OTP", "Invalid OTP"));
       
        if (otp.Type != otpType) return new(new OperationFailedException("OTP", "Invalid OTP type"));

        if (otp.ValidUntil < DateTimeOffset.UtcNow || otp.Verified)
            return new(new OperationFailedException("OTP", "OTP has expired. Request for a new OTP"));

        otp.MarkAsVerified();

        return new(new OTPDto(otp.Id, otpType, identityNumber, otp.SentTo));
    }
}
