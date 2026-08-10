using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class PassCategoryPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.CreatePassCategory,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.CreatePassCategory));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.ViewPassCategory,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.ViewPassCategory));
                });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.ViewPassRate,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.ViewPassRate));
                });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.EditPassRate,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.EditPassRate));
                });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.EditPassCategory,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.EditPassCategory));
                });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.DeletePassCategory,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.DeletePassCategory));
                });

        options.AddPolicy(ApplicationAuthPolicy.PassCategoryPolicy.Manage,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassCategory.ManagePassTemplate));
                });
    }
}
