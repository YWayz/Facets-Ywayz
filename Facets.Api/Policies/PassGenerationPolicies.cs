using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public class PassGenerationPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.PassGenerationPolicy.PassGenerationView,
                       policy =>
                       {
                           policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassGeneration.PassGenerationView));
                       });

        options.AddPolicy(ApplicationAuthPolicy.PassGenerationPolicy.VisitorPassGeneration,
               policy =>
               {
                   policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassGeneration.VisitorPassGeneration));
               });

        options.AddPolicy(ApplicationAuthPolicy.PassGenerationPolicy.TeamMemberPassGeneration,
         policy =>
         {
             policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassGeneration.TeamMemberPassGeneration));
         });

        options.AddPolicy(ApplicationAuthPolicy.PassGenerationPolicy.PassVerification,
        policy =>
        {
            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PassGeneration.PassVerification));
        });
    }
}
