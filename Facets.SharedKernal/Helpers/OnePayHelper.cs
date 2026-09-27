using NanoidDotNet;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Facets.SharedKernal.Helpers;

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

    public static string GenerateReferenceNumber()
    {
        return Nanoid.Generate(alphabet: NanoidAlphabets, size: 20);
    }
}
