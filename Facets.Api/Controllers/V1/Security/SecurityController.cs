using Facets.Core.Security.Dtos;
using Facets.Core.Security.Interfaces;
using Facets.Core.Security.ModulePermissions;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Security;

[Route("api/security")]
public sealed class SecurityController : AdminAppControllerBase
{
    private readonly ISecurityService _securityService;

    public SecurityController(ISecurityService securityService)
    {
        _securityService = securityService;
    }

    /// <summary>
    /// Authenticate a user by providing the username and password
    /// </summary>
    /// <param name="model"></param>
    /// <returns>A Respoonse result object conataining the authenticated user info</returns>
    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(ResponseResult<AuthenticatedUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> Authenticate([FromBody] AuthenticateUserDto model)
    {
        var response = await _securityService.AuthenticateUser(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost("logout")]
    [ProducesResponseType(typeof(ResponseResult<AuthenticatedUserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> LogOut([FromBody] UnAuthenticateUserDto model)
    {
        var response = await _securityService.LogOut(User, model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("app-permissions")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<string, IReadOnlyList<PermissionSet>>>>), StatusCodes.Status200OK)]
    public ActionResult GeApplicationPermissions()
    {
        var permissionList = AppModulePermissions.GetPermissionList();

        var response = new ResponseResult<IReadOnlyList<KeyValuePair<string, IReadOnlyList<PermissionSet>>>>(permissionList, permissionList.Count);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> SendPasswordResetEmail([FromBody] ForgotPasswordModel forgotPasswordModel)
    {
        var response = await _securityService.SendResetPasswordEmail(forgotPasswordModel, CancellationToken.None);

        return response.Success ? Ok() : UnsuccessfullResponse(response);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var response = await _securityService.ResetPassword(model, CancellationToken.None);

        return response.Success ? Ok() : UnsuccessfullResponse(response);
    }
}
