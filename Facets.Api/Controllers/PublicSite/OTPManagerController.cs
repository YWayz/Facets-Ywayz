using Facets.Api.DIServiceExtensions;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.Security.Dtos;
using Facets.Core.Security.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/otp-manager")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class OTPManagerController : PublicAppControllerBase
{
    private readonly IOTPStore _otpStore;

    public OTPManagerController(IOTPStore otpStore)
    {
        _otpStore = otpStore;
    }

    [HttpPost("send")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingConfig.OtpPolicy)]
    [ProducesResponseType(typeof(ResponseResult<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GenerateOTP([FromBody] GenerateOTPDto model)
    {
        var response = await _otpStore.GenerateOTP(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    [EnableRateLimiting(RateLimitingConfig.OtpPolicy)]
    [ProducesResponseType(typeof(ResponseResult<PublicUserAuthenticatedDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> VerifyOTP([FromBody] VerifyOTPDto model)
    {
        var response = await _otpStore.VerifyOTP(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
