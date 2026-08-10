using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies;

public sealed class PavilionPolicies : IAuthPolicyApplyer
{
    public void Apply(AuthorizationOptions options)
    {
        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.Create,
                       policy =>
                       {
                           policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.Create));
                       });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.Delete,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.Delete));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.Edit,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.Edit));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.View,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.View));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.TogglePavilionStatus,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.TogglePavilionStatus));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.CreatePavilionSession,
               policy =>
               {
                   policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.CreatePavilionSession));
               });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.EditPavilionSession,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.EditPavilionSession));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.ViewPavilionSession,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.ViewPavilionSession));
                        });
        
        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.SessionDelete,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.SessionDelete));
                        });

        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.RateView,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.RateView));
                        });
        
        options.AddPolicy(ApplicationAuthPolicy.PavilionPolicy.EditPassPavilionRate,
                        policy =>
                        {
                            policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Pavilion.EditPassPavilionRate));
                        });
    }
}
