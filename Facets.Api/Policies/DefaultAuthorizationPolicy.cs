using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Facets.Api.Policies;

/// <summary>
/// The policy applied to every endpoint that does not name its own (via MapControllers().RequireAuthorization()).
///
/// It used to be "any authenticated user". A visitor who verifies an OTP on the public site receives a
/// JWT and is therefore authenticated, so that visitor could call every admin endpoint that had no
/// explicit policy: visitor reports, staff lookups, QR attendance verification and more.
///
/// Now a public-site (visitor) token is accepted only on endpoints that either carry
/// [Authorize(policy: PublicSiteUser)] or are marked [AllowPublicSiteUser]. Staff tokens are unaffected
/// and still go through the existing claim and event-access policies.
/// </summary>
public static class DefaultAuthorizationPolicy
{
    public static AuthorizationPolicy Build()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(StaffOnlyUnlessEndpointAllowsVisitors)
            .Build();
    }

    private static bool StaffOnlyUnlessEndpointAllowsVisitors(AuthorizationHandlerContext context)
    {
        if (IsPublicSiteUser(context.User) is false) return true;

        var httpContext = context.Resource as HttpContext;
        var endpoint = httpContext?.GetEndpoint();

        if (endpoint is null) return false;

        if (endpoint.Metadata.GetMetadata<AllowPublicSiteUserAttribute>() is not null) return true;

        return endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>()
                                .Any(a => string.Equals(a.Policy, ApplicationAuthPolicy.PublicSiteUser, StringComparison.Ordinal));
    }

    private static bool IsPublicSiteUser(ClaimsPrincipal user)
    {
        string? subject = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return subject == AppConstants.PublicSite.PublicSiteUserId;
    }
}
