using Facets.Core.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Facets.Api.PolicyRequriements.PublicSiteUserAccessRequirements;

public sealed class PublicSiteUserAccessRequirementHandler : AuthorizationHandler<PublicSiteUserAccessRequirement>
{
    private readonly IUserSecurityRespository _userSecurityRespository;

    public PublicSiteUserAccessRequirementHandler(IUserSecurityRespository userSecurityRespository)
    {
        _userSecurityRespository = userSecurityRespository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PublicSiteUserAccessRequirement requirement)
    {

        if (context.User.Identity!.IsAuthenticated is false)
        {
            context.Fail();
            return;
        }

        var jti = context?.User?.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Jti)?.Value;
        var identificationNumber = context?.User?.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Name)?.Value;

        if (string.IsNullOrWhiteSpace(jti))
        {
            context!.Fail();
            return;
        }

        bool isTokenValid = await _userSecurityRespository.VerifyToken(identificationNumber, jti);

        if (isTokenValid is true)
        {

            context!.Succeed(requirement);
            return;
        }

        context!.Fail();
        return;
    }
}
