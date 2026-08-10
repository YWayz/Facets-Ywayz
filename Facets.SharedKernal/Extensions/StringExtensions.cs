namespace Facets.SharedKernal.Extensions;
public static class StringExtensions
{
    public static string RemoveWhitespaces(this string value)
    {
        return value.Replace(" ", "").Trim();
    }
}
