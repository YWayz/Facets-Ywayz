using Facets.Core.Security.Dtos;
using Facets.Core.Security.Entities;
using Facets.Core.Security.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using NanoidDotNet;
using System.Security.Cryptography;
using System.Text;
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
        // Cap how many OTPs one identity can request, otherwise requesting a fresh OTP
        // would reset the failed-attempt counter and allow unlimited guessing.
        var windowStart = DateTimeOffset.UtcNow.AddMinutes(-AppConstants.OTP.SendWindowMinutes);

        int recentlySent = await _otpRepository.CountSentSince(identityNumber, windowStart);

        if (recentlySent >= AppConstants.OTP.MaxSendsPerWindow)
            return new(new OperationFailedException("OTP", "Too many OTP requests. Please try again later."));

        var code = await Nanoid.GenerateAsync(AppConstants.OTP.Characters, AppConstants.OTP.Length);

        OTPQueue otp = new(code, type, identityNumber, sendTo);

        _otpRepository.Add(otp);

        return new(new OTPInternalDto(otp.Id, otp.Type, otp.IdentityNumber, otp.SentTo, otp.Code));
    }

    /// <remarks>
    /// On a wrong code this increments the OTP's failed-attempt counter. The caller must save
    /// changes on failure as well as on success, or the counter is lost.
    /// </remarks>
    public async Task<ResponseResult<OTPDto>> VerifyOTP(string identityNumber, string code, OTPType otpType)
    {
        var otp = await _otpRepository.GetOTPToVerify(identityNumber);

        if (otp is null) return new(new OperationFailedException("OTP", "Invalid OTP"));

        if (otp.Verified || otp.ValidUntil < DateTimeOffset.UtcNow)
            return new(new OperationFailedException("OTP", "OTP has expired. Request for a new OTP"));

        if (otp.IsLocked)
            return new(new OperationFailedException("OTP", "Too many incorrect attempts. Request for a new OTP"));

        if (CodesMatch(otp.Code, code) is false)
        {
            otp.RegisterFailedAttempt();

            if (otp.IsLocked)
                return new(new OperationFailedException("OTP", "Too many incorrect attempts. Request for a new OTP"));

            return new(new OperationFailedException("OTP", "Invalid OTP"));
        }

        if (otp.Type != otpType) return new(new OperationFailedException("OTP", "Invalid OTP type"));

        otp.MarkAsVerified();

        return new(new OTPDto(otp.Id, otpType, identityNumber, otp.SentTo));
    }

    private static bool CodesMatch(string expected, string? actual)
    {
        if (actual is null) return false;

        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(actual));
    }
}
