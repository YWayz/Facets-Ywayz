using NanoidDotNet;
using System.Security.Cryptography;
using System.Text;

namespace Facets.SharedKernal.Helpers;

public static class OnePayHelper
{
    private const string NanoidAlphabets = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static SHA256 Hasher = SHA256.Create();

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
        return totalAmount.ToString("0.00");
    }

    public static string GenerateReferenceNumber()
    {
        return Nanoid.Generate(alphabet: NanoidAlphabets, size: 20);
    }
}
