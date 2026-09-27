namespace Facets.Api.DIServiceExtensions;

public static class CorsConfig
{
    // Used when the "Cors:AllowedOrigins" setting is not provided.
    private static readonly string[] DefaultOrigins =
    {
        "https://exhibition.facetssrilanka.com",
        "https://facets-uat.azurewebsites.net",
    };

    private static readonly string[] DevelopmentOrigins =
    {
        "http://localhost:4200",
        "https://localhost:4200",
        "https://localhost:44335",
    };

    /// <summary>
    /// Allowed origins come from configuration so a new App Service or custom domain only needs an
    /// app setting, not a code change: set "Cors__AllowedOrigins" to a comma-separated list, e.g.
    /// "https://exhibition.facetssrilanka.com,https://my-app.azurewebsites.net".
    /// Localhost origins are only allowed in Development.
    /// </summary>
    public static IServiceCollection AddCorsConfig(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        string? configured = configuration["Cors:AllowedOrigins"];

        var origins = string.IsNullOrWhiteSpace(configured)
            ? DefaultOrigins.ToList()
            : configured.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(o => o.TrimEnd('/'))
                        .ToList();

        if (environment.IsDevelopment()) origins.AddRange(DevelopmentOrigins);

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                .WithOrigins(origins.Distinct(StringComparer.OrdinalIgnoreCase).ToArray())
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }
}
