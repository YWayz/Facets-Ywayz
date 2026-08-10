using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Events;

[Route("api/attendees")]
public sealed class EventAttendeesController : AdminAppControllerBase
{
    private readonly IEventVisitorService _eventVisitorService;

    public EventAttendeesController(IEventVisitorService eventVisitorService)
    {
        _eventVisitorService = eventVisitorService;
    }

    [HttpGet("~/api/event-dates/{eventDateId}/attendees")]
    [Authorize(policy: ApplicationAuthPolicy.PassGenerationPolicy.PassGenerationView)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<Attendee>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetEventVisitors([FromRoute] Guid eventDateId, [FromQuery] Paginator paginator, [FromQuery] AttendeesFilter filter, CancellationToken token)
    {
        var response = await _eventVisitorService.GetEventVisitors(eventDateId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("~/api/pavilion-sessions")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionVisitorPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyCollection<PavilionSessionVisitorDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilionVisitors([FromServices] IVisitorPavilionService visitorPavilionService, [FromQuery] Paginator paginator, [FromQuery] PavilionSessionVisitorFilter filter, CancellationToken token)
    {
        var response = await visitorPavilionService.GetPavilionSessionVisitors(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
