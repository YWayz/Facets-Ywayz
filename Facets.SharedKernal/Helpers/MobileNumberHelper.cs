namespace Facets.SharedKernal.Helpers;

public static class MobileNumberHelper
{
    public static readonly string[] SLCodes = { "0094", "+94", "94" };

    public static bool IsSriLankaNumber(string mobileNumber)
    {
        return SLCodes.Any(c => mobileNumber.StartsWith(c)) ||
               mobileNumber.Length <= AppConstants.TextIt.MaximumLengthOfSriLankaPhoneNumberWithoutCountryCode;
    }
}
