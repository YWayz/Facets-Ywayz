using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Passes.DTOs;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Passes;

[Route("api/visitor-passes")]
public sealed class VisitorPassesController : AdminAppControllerBase
{
    private readonly IEventVisitorService _eventVisitorService;

    public VisitorPassesController(IEventVisitorService eventVisitorService)
    {
        _eventVisitorService = eventVisitorService;
    }

    [HttpGet("verification-details/event/{eventId}/{visitorId}/{eventDateId}")]
    [Authorize(policy: ApplicationAuthPolicy.PassGenerationPolicy.VisitorPassGeneration)]
    [ProducesResponseType(typeof(ResponseResult<VisitorPassVerificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitorVerificationDetails([FromRoute] Guid eventId, [FromRoute] Guid visitorId, [FromRoute] Guid eventDateId, CancellationToken token)
    {
        var response = await _eventVisitorService.GetVisitorVerificationDetails(eventId, visitorId, eventDateId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("template/{eventId}/{visitorId}/{eventDateId}")]
    [Authorize(policy: ApplicationAuthPolicy.PassGenerationPolicy.VisitorPassGeneration)]
    [ProducesResponseType(typeof(ResponseResult<VisitorPassTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitorPassGenerationTemplate([FromRoute] Guid eventId, [FromRoute] Guid visitorId, [FromRoute] Guid eventDateId, CancellationToken token)
    {
        var response = await _eventVisitorService.GetVisitorPassGenerationTemplate(eventId, visitorId, eventDateId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("verify-qr")]
    [ProducesResponseType(typeof(ResponseResult<QRVerifiedVisitorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> VerifyByQR([FromBody] VisitorPassVerificationDto model)
    {
        var response = await _eventVisitorService.VerifyPass(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("verify-qr/pavilion-session")]
    [ProducesResponseType(typeof(ResponseResult<QRVerifiedVisitorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> VerifyPavilionSessionByQR([FromBody] VisitorPavilionPassVerificationDto model)
    {
        var response = await _eventVisitorService.VerifyPavilionPass(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}