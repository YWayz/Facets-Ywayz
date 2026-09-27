using Facets.Core.Security.Entities;

namespace Facets.Core.Security.Interfaces;

public interface IOTPRepository
{
    OTPQueue Add(OTPQueue otp);
    Task<OTPQueue?> GetOTPToVerify(string identityNumber);
    Task<int> CountSentSince(string identityNumber, DateTimeOffset since);
}
