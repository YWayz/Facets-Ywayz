namespace Facets.Api.DIServiceExtensions;

/// <summary>
/// Checks at startup that every setting the API needs is present, and stops with one clear
/// message naming the missing ones.
///
/// Why: locally these come from User Secrets, which .NET only loads in the Development environment.
/// On Azure they must be App Service settings. When they were missing, the app started anyway and
/// then failed on every request with unrelated-looking errors (null connection strings, empty JWT key).
/// </summary>
public static class RequiredConfigValidation
{
    // Key as read by the code -> name to use in Azure App Service "Environment variables".
    private static readonly (string Key, string AzureName)[] RequiredSettings =
    {
        ("ConnectionStrings:MSSQLDbConnection", "Connection string: MSSQLDbConnection"),
        ("ConnectionStrings:AzureStorage",      "Connection string: AzureStorage"),
        ("JwtConfig:Issuer",                    "JwtConfig__Issuer"),
        ("JwtConfig:Audience",                  "JwtConfig__Audience"),
        ("JwtConfig:SigningKey",                "JwtConfig__SigningKey"),
        ("JwtConfig:TokenLifetime",             "JwtConfig__TokenLifetime"),
        ("OnePaySettings:BaseURL",              "OnePaySettings__BaseURL"),
        ("OnePaySettings:AppID",                "OnePaySettings__AppID"),
        ("OnePaySettings:AppToken",             "OnePaySettings__AppToken"),
        ("OnePaySettings:HashSalt",             "OnePaySettings__HashSalt"),
        ("OnePaySettings:PaymentRequestEndPoint", "OnePaySettings__PaymentRequestEndPoint"),
        ("OnePaySettings:TransactionRedirectUrl", "OnePaySettings__TransactionRedirectUrl"),
    };

    // Only some features use these, so a missing one is logged as a warning rather than stopping startup.
    private static readonly (string Key, string AzureName)[] OptionalSettings =
    {
        ("Assocify:BaseURL",                             "Assocify__BaseURL"),
        ("Assocify:TenantId",                            "Assocify__TenantId"),
        ("Assocify:FuncAppKeys:GetMemberBySearchValue",  "Assocify__FuncAppKeys__GetMemberBySearchValue"),
        ("Assocify:FuncAppKeys:IsMemberAvailable",       "Assocify__FuncAppKeys__IsMemberAvailable"),
    };

    public static void ValidateRequiredConfiguration(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        var missing = RequiredSettings.Where(s => string.IsNullOrWhiteSpace(configuration[s.Key])).ToList();
        var missingOptional = OptionalSettings.Where(s => string.IsNullOrWhiteSpace(configuration[s.Key])).ToList();

        if (missingOptional.Count > 0)
        {
            Serilog.Log.Warning("Optional settings are missing (Assocify member lookup will not work): {Settings}",
                                string.Join(", ", missingOptional.Select(s => s.AzureName)));
        }

        if (missing.Count == 0) return;

        string message =
            $"Facets.Api cannot start: {missing.Count} required setting(s) are missing in environment '{builder.Environment.EnvironmentName}'. " +
            "Locally these come from User Secrets (Development only); on Azure add them under App Service > Settings > Environment variables: " +
            string.Join(", ", missing.Select(s => s.AzureName)) +
            ". See DEPLOYMENT.md.";

        Serilog.Log.Fatal(message);
        Console.Error.WriteLine(message);

        throw new InvalidOperationException(message);
    }
}
