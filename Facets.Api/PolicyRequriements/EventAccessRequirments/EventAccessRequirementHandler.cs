using Facets.Core.Security.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Facets.Api.PolicyRequriements.EventAccessRequirments;

public sealed class EventAccessRequirementHandler : AuthorizationHandler<EventAccessRequirement>
{
    private readonly ILoggedInUserService _loggedInUserService;
    private readonly IUserSecurityRespository _userSecurityRespository;

    public EventAccessRequirementHandler(ILoggedInUserService loggedInUserService, IUserSecurityRespository userSecurityRespository)
    {
        _loggedInUserService = loggedInUserService;
        _userSecurityRespository = userSecurityRespository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EventAccessRequirement requirement)
    {
        if (context.User.Identity!.IsAuthenticated is false)
        {
            context.Fail();
            return;
        }

        var userEmail = _loggedInUserService.UserEmail;

        if (userEmail is null)
        {
            context.Fail();
            return;
        }

        var user = await _userSecurityRespository.GetUser(context.User);

        if (user is null)
        {
            context.Fail();
            return;
        }


        bool isSuperAdmin = _loggedInUserService.UserId == AppConstants.SuperAdmin.SuperUserId.ToString();

        if (isSuperAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        bool hasAccessToEvent = await _userSecurityRespository.HasAccessToEvent(user.Id, _loggedInUserService.FacetsEventId);

        if (hasAccessToEvent)
        {
            context.Succeed(requirement);
            return;
        }

        else
        {
            context.Fail();
        }
    }
}
