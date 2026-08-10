namespace Facets.Api.DIServiceExtensions;

public static class CorsConfig
{
    public static IServiceCollection AddCorsConfig(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                .WithOrigins("http://localhost:4200", "https://localhost:44335", "https://localhost:4200", "https://exhibition.facetssrilanka.com", "http://exhibition.facetssrilanka.com", "https://facets-uat.azurewebsites.net", "http://facets-uat.azurewebsites.net")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }
}
