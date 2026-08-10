using Facets.Api.OutputCachePolicies;
using static Facets.Api.APIConstants;

namespace Facets.Api.DIServiceExtensions;

public static class OutputCacheConfig
{
    public static IServiceCollection AddOutputCacheConfig(this IServiceCollection services)
    {
        services.AddOutputCache(opt =>
        {
            opt.AddPolicy(OutputCachePolicyNames.CountryCachePolicy, policy =>
            {
                policy.AddPolicy<LookupOutputCachePolicy>()
                      .Expire(TimeSpan.FromDays(4));
            }, excludeDefaultPolicy: true);

            opt.AddPolicy(OutputCachePolicyNames.PassCategoryCachePolicy, policy =>
            {
                policy.AddPolicy<LookupOutputCachePolicy>()
                      .Expire(TimeSpan.FromHours(6))
                      .Tag(OutputCachePolicyNames.PassCategoryCachePolicy);
            }, excludeDefaultPolicy: true);
        });

        return services;
    }
}
