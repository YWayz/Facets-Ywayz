using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class PaymentSettingsPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.PaymentSettingPolicy.PaymentSettingsUpdate,
             policy =>
             {
                 policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.PaymentSetting.PaymentSettingsUpdate));
             });
    }
}
