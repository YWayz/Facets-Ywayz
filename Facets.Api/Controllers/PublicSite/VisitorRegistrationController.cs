using Facets.Api.Services;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/registrations")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class VisitorRegistrationController : PublicAppControllerBase
{
    private readonly IVisitorRegistrationStore _visitorRegistrationStore;
    private readonly IVisitorRegistrationService _visitorRegistrationService;
    private readonly IPublicSiteOwnership _ownership;

    public VisitorRegistrationController(IVisitorRegistrationStore visitorRegistrationStore, IVisitorRegistrationService visitorRegistrationService, IPublicSiteOwnership ownership)
    {
        _visitorRegistrationStore = visitorRegistrationStore;
        _visitorRegistrationService = visitorRegistrationService;
        _ownership = ownership;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> RegisterOnlineVisitorToEvent([FromBody] VisitorOnlineEventRegistrationDto model)
    {
        if (await _ownership.OwnsVisitor(model.VisitorId, CancellationToken.None) is false) return NotOwnedResponse("Visitor", model.VisitorId);

        var response = await _visitorRegistrationStore.RegisterOnlineVisitorToEvent(model, CancellationToken.None);
        return response.Success ? CreatedAtRoute(nameof(GetPublicRegistrationById), new { id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetPublicRegistrationById))]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPublicRegistrationById([FromRoute] Guid id, CancellationToken token)
    {
        if (await _ownership.OwnsRegistration(id, token) is false) return NotOwnedResponse("Registration", id);

        var response = await _visitorRegistrationService.GetRegistrationById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorRegistrationSummary>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRegistrations([FromQuery] Paginator paginator, [FromQuery] RegistrationFilter filter, CancellationToken token)
    {
        // Always limited to the signed-in visitor's own registrations, whatever visitorId was asked for.
        var ownVisitorId = await _ownership.GetOwnVisitorId(token);

        if (ownVisitorId is null) return NotOwnedResponse("Visitor", _ownership.IdentityNumber ?? string.Empty);

        filter = filter with { visitorId = ownVisitorId };

        var response = await _visitorRegistrationService.GetRegistrations(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("~/api/" + APIConstants.APIGroup.Public + "/visitors/{visitorId}/registration")]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVisitorRegistrationRegistration([FromRoute] Guid visitorId, CancellationToken token)
    {
        if (await _ownership.OwnsVisitor(visitorId, token) is false) return NotOwnedResponse("Visitor", visitorId);

        var response = await _visitorRegistrationService.GetVisitorRegistration(visitorId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateOnlineVisitorRegistration([FromRoute] Guid id, [FromBody] UpdateVisitorOnlineEventRegistrationDto model)
    {
        if (await _ownership.OwnsRegistration(id, CancellationToken.None) is false) return NotOwnedResponse("Registration", id);

        var response = await _visitorRegistrationStore.UpdateOnlineVisitorRegistration(id, model, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/cancel-attendance-schedules")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CancelOnsiteVisitorAttendance([FromRoute] Guid id, [FromBody] CancelVisitorAttendanceDto model)
    {
        if (await _ownership.OwnsRegistration(id, CancellationToken.None) is false) return NotOwnedResponse("Registration", id);

        var response = await _visitorRegistrationStore.CancelOnlineVisitorAttendance(id, model, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
