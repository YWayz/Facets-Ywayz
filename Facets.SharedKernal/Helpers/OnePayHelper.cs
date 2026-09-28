using NanoidDotNet;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Facets.SharedKernal.Helpers;

public static class IdentityNumberHelper
{
    /// <summary>
    /// One canonical form for NIC and passport numbers: trimmed, inner whitespace removed, upper-case.
    /// Everything that stores, looks up or compares an identity number must go through this, otherwise
    /// " 123456789V" and "123456789v" become different people to one part of the system and the same to another.
    /// </summary>
    public static string? Normalize(string? identityNumber)
    {
        if (string.IsNullOrWhiteSpace(identityNumber)) return null;

        return string.Concat(identityNumber.Where(c => char.IsWhiteSpace(c) is false)).ToUpperInvariant();
    }
}

public static class OnePayHelper
{
    private const string NanoidAlphabets = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string ComputeSHA256(string hashSalt, string value)
    {
        var valueToHash = $"{value}{hashSalt}";

        var contentBytes = Encoding.UTF8.GetBytes(valueToHash);

        byte[] hash;

        using (var sha256 = SHA256.Create())
        {
            hash = sha256.ComputeHash(contentBytes);

        }

        StringBuilder hashStringBuilder = new StringBuilder();
       
        foreach (byte b in hash)
        {
            hashStringBuilder.Append(b.ToString("x2"));
        }

        var hashedValue = hashStringBuilder.ToString();
        
        return hashedValue;
    }

    public static string FormatAmount(decimal totalAmount)
    {
        // Always invariant: the server culture must never change the amount we send or hash.
        return RoundAmount(totalAmount).ToString("0.00", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Rounds to 2 decimal places and forces a scale of 2 (1500 -> 1500.00), so the amount
    /// serializes to JSON exactly as it is written into the request hash.
    /// </summary>
    public static decimal RoundAmount(decimal totalAmount)
    {
        return Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero) + 0.00m;
    }

    /// <summary>
    /// OnePay v3 expects customer_phone_number in E.164 form (+94771234567). Visitors type numbers as
    /// 0771234567, 94771234567, 0094771234567 or +94 77 123 4567; all of these become +94771234567.
    /// Non-Sri-Lankan numbers keep their own country code.
    /// </summary>
    public static string ToE164(string? mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber)) return string.Empty;

        string digits = string.Concat(mobileNumber.Where(char.IsDigit));

        if (digits.StartsWith("00")) digits = digits[2..];

        if (digits.Length == 10 && digits.StartsWith("0")) return "+94" + digits[1..];

        if (digits.Length == 9 && mobileNumber.Trim().StartsWith("+") is false && digits.StartsWith("7")) return "+94" + digits;

        return "+" + digits;
    }

    public static string GenerateReferenceNumber()
    {
        return Nanoid.Generate(alphabet: NanoidAlphabets, size: 20);
    }
}
