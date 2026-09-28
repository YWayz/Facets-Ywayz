using Facets.Core.Common.Dtos;
using Facets.Core.Security.Dtos;
using Facets.Core.Security.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Security;

[Route("api/security/users/{userId}/profile")]
[ApiController]
public sealed class UserProfileController : AdminAppControllerBase
{
    private readonly ISecurityService _securityService;
    private readonly ILoggedInUserService _loggedInUser;

    public UserProfileController(ISecurityService securityService, ILoggedInUserService loggedInUser)
    {
        _securityService = securityService;
        _loggedInUser = loggedInUser;
    }

    // A staff member may only read or change their own profile and password; administrators may manage anyone's.
    private bool CanAccess(Guid userId) => _loggedInUser.IsAdminUser() || string.Equals(_loggedInUser.UserId, userId.ToString(), StringComparison.OrdinalIgnoreCase);

    private ObjectResult NotYourProfile() => UnsuccessfullResponse(new ResponseResult(new NotFoundException("userId", "User", "profile")));

    [HttpPut("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeUserPassword([FromRoute] Guid userId, [FromBody] UpdateUserPasswordDto model)
    {
        if (CanAccess(userId) is false) return NotYourProfile();

        var response = await _securityService.ChangeUserPassword(userId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserProfileById([FromRoute] Guid userId, CancellationToken token)
    {
        if (CanAccess(userId) is false) return NotYourProfile();

        var response = await _securityService.GetUserProfile(userId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateUserProfile([FromRoute] Guid userId, [FromBody] UpdateUserProfileDto model)
    {
        if (CanAccess(userId) is false) return NotYourProfile();

        var response = await _securityService.UpdateUserProfile(userId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPost("image")]
    [ProducesResponseType(typeof(ResponseResult<FileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddProfileImage([FromRoute] Guid userId, IFormFile file)
    {
        if (CanAccess(userId) is false) return NotYourProfile();

        var response = await _securityService.AddProfileImageToExistingUser(userId, file, CancellationToken.None);

        return response.Success ? Created(response.Data!.URI, response) : UnsuccessfullResponse(response);
    }

    [HttpDelete("image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveProfileImage([FromRoute] Guid userId)
    {
        if (CanAccess(userId) is false) return NotYourProfile();

        var response = await _securityService.RemoveUserProfileImage(userId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}


