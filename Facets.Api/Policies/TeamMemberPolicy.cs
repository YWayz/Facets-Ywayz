using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class TeamMemberPolicy : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.Register,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.Register));
            });
        
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.UploadAttachments,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.UploadAttachments));
            });
        
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.ViewAttachments,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.ViewAttachments));
            });
        
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.View,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.View));
            });

        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.UpdateTeamMember,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.UpdateTeamMember));
            });
        
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.CancelTeamMemberRegistration,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.CancelTeamMemberRegistration));
            });
        options.AddPolicy(ApplicationAuthPolicy.TeamMemberPolicy.GenerateTeamMemberPass,
            policy =>
            {
                policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.TeamMemberRegistration.GenerateTeamMemberPass));
            });
    }
}
