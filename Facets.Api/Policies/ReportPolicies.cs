using Facets.Api.PolicyRequriements.UserClaimRequirements;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.Policies
{
    public class ReportPolicies : IAuthPolicyApplyer
    {
        public void Apply(AuthorizationOptions options)
        {
            options.AddPolicy(ApplicationAuthPolicy.ReportPolicy.CollectionReport,
                       policy =>
                       {
                           policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Report.GenerateCollectionReport));
                       });

            options.AddPolicy(ApplicationAuthPolicy.ReportPolicy.VisitorReport,
                     policy =>
                     {
                         policy.Requirements.Add(new UserClaimRequirement(ApplicationClaimValues.Report.GenerateVisitorListReport));
                     });
        }
    }
}
