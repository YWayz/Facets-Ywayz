using Facets.Api.PolicyRequriements.EventAccessRequirments;
using Facets.Api.PolicyRequriements.PublicSiteUserAccessRequirements;
using Facets.Core.Security.AuthPolicies;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class PublicSiteUserAccessPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.PublicSiteUser,
                    policy =>
                    {
                        policy.Requirements.Add(new PublicSiteUserAccessRequirement());
                    });
    }
}
