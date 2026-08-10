using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Registrations;

[Route("api/registrations")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class VisitorRegistrationController : AdminAppControllerBase
{
    private readonly IVisitorRegistrationStore _visitorRegistrationStore;
    private readonly IVisitorRegistrationService _visitorRegistrationService;

    public VisitorRegistrationController(IVisitorRegistrationStore visitorRegistrationStore, IVisitorRegistrationService visitorRegistrationService)
    {
        _visitorRegistrationStore = visitorRegistrationStore;
        _visitorRegistrationService = visitorRegistrationService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.OnsiteRegistration)]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> RegisterOnsiteVisitorToEvent([FromBody] VisitorOnsiteEventRegistrationDto model)
    {
        var response = await _visitorRegistrationStore.RegisterOnsiteVisitorToEvent(model, CancellationToken.None);
        return response.Success ? CreatedAtRoute(nameof(GetRegistrationById), new { id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetRegistrationById))]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRegistrationById([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _visitorRegistrationService.GetRegistrationById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorRegistrationSummary>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRegistrations([FromQuery] Paginator paginator, [FromQuery] RegistrationFilter filter, CancellationToken token)
    {
        var response = await _visitorRegistrationService.GetRegistrations(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("~/api/visitors/{visitorId}/registration")]
    [ProducesResponseType(typeof(ResponseResult<RegisteredVisitorDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVisitorRegistrationRegistration([FromRoute] Guid visitorId, CancellationToken token)
    {
        var response = await _visitorRegistrationService.GetVisitorRegistration(visitorId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateOnsiteVisitorRegistration([FromRoute] Guid id, [FromBody] UpdateVisitorOnsiteEventRegistrationDto model)
    {
        var response = await _visitorRegistrationStore.UpdateOnsiteVisitorRegistration(id, model, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/cancel-attendance-schedules")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.CancelRegistration)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CancelOnsiteVisitorAttendance([FromRoute] Guid id, [FromBody] CancelVisitorAttendanceDto model)
    {
        var response = await _visitorRegistrationStore.CancelOnsiteVisitorAttendance(id, model, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("{visitorId}/pass/registrations/{visitorRegistrationId}")]
    [ProducesResponseType(typeof(ResponseResult<VisitorPassRegistrationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetVisitorPassRegistration([FromRoute] Guid visitorId, [FromRoute] Guid visitorRegistrationId, [FromQuery] RegistrationFilter filter, CancellationToken token)
    {
        var response = await _visitorRegistrationService.GetVisitorPassRegistration(visitorId, visitorRegistrationId, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("~/api/visitors/{visitorId}/attendance/{attendanceScheduleId}/mark-pass-as-printed")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkPassAsPrinted([FromRoute] Guid visitorId, [FromRoute] Guid attendanceScheduleId)
    {
        var response = await _visitorRegistrationService.MarkPassAsPrinted(visitorId, attendanceScheduleId, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
