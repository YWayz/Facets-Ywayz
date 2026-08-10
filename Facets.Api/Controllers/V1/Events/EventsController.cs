using Facets.Core.Common.Dtos;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Filters;
using Facets.Core.Events.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Events;

[Route("api/events")]
public sealed class EventsController : AdminAppControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.EventPolicy.Create)]
    [ProducesResponseType(typeof(ResponseResult<EventDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateEvent([FromBody] CreateEventDto model)
    {
        var response = await _eventService.CreateEvent(model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetEventById), new { id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetEventById))]
    [ProducesResponseType(typeof(ResponseResult<EventDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetEventById([FromRoute] Guid id, CancellationToken token)
    {
        var response = await _eventService.GetEventById(id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost("{eventId}/logo")]
    [ProducesResponseType(typeof(ResponseResult<FileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddLogoToExistingEvent([FromRoute] Guid eventId, IFormFile file)
    {
        var response = await _eventService.AddEventLogo(eventId, file, CancellationToken.None);

        return response.Success ? Created(response.Data!.URI, response) : UnsuccessfullResponse(response);
    }

    [HttpDelete("{eventId}/logo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEventLogo([FromRoute] Guid eventId)
    {
        var response = await _eventService.DeleteEventLogo(eventId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.EventPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<EventSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetEvents([FromQuery] Paginator paginator, [FromQuery] EventFilter filter, CancellationToken token)
    {
        var response = await _eventService.GetEvents(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{eventId}")]
    [Authorize(policy: ApplicationAuthPolicy.EventPolicy.Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateEvent([FromRoute] Guid eventId, [FromBody] UpdateEventDto model)
    {
        var response = await _eventService.UpdateEvent(eventId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{eventId}")]
    [Authorize(policy: ApplicationAuthPolicy.EventPolicy.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteEvent([FromRoute] Guid eventId)
    {
        var response = await _eventService.Delete(eventId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{eventId}/status")]
    [Authorize(policy: ApplicationAuthPolicy.EventPolicy.ToggleEventStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateEventStatus([FromRoute] Guid eventId, [FromBody] UpdateEventStatusDto model)
    {
        var response = await _eventService.UpdateEventStatus(eventId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
