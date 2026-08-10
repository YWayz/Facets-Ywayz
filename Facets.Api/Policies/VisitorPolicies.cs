using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class VisitorPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.VisitorPolicy.OnsiteRegistration,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.VisitorRegistration.OnsiteRegister));
                        });

        options.AddPolicy(ApplicationAuthPolicy.VisitorPolicy.OnsiteUpdate,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.VisitorRegistration.OnsiteUpdate));
                        });

        options.AddPolicy(ApplicationAuthPolicy.VisitorPolicy.View,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.VisitorRegistration.View));
                        });

        options.AddPolicy(ApplicationAuthPolicy.VisitorPolicy.Blacklist,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Visitor.BlackList));
                        });

        options.AddPolicy(ApplicationAuthPolicy.VisitorPolicy.CancelRegistration,
                policy =>
                {
                    policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.VisitorRegistration.CancelVisitorRegistration));
                });
    }
}
