using Facets.Core.Events.DTOs;
using Facets.Core.Events.Filters;
using Facets.Core.Events.Interfaces;
using Facets.Core.Events.Specs;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Events;

[Route("api/events/{eventId}/pavilions")]
public sealed class PavlilionsController : AdminAppControllerBase
{
    private readonly IPavilionService _pavilionService;

    public PavlilionsController(IPavilionService pavilionService)
    {
        _pavilionService = pavilionService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.Create)]
    [ProducesResponseType(typeof(ResponseResult<PavilionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePavilion([FromRoute] Guid eventId, [FromBody] CreatePavilionDto model)
    {
        var response = await _pavilionService.CreatePavilion(eventId, model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetPavilionById), new { eventId = eventId, id = response.Data!.Id }, response) :
                                  UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PavilionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilions([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] PavilionFilter filter, CancellationToken token)
    {
        var response = await _pavilionService.GetPavilions(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetPavilionById))]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<PavilionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPavilionById([FromRoute] Guid eventId, [FromRoute] Guid id, CancellationToken token)
    {
        var response = await _pavilionService.GetPavilionById(eventId, id, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePavilion([FromRoute] Guid eventId, [FromRoute] Guid id, [FromBody] UpdatePavilionDto model)
    {
        var response = await _pavilionService.UpdatePavilion(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/status")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.TogglePavilionStatus)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePavilionStatus([FromRoute] Guid eventId, [FromRoute] Guid id, [FromBody] UpdatePavilionStatusDto model)
    {
        var response = await _pavilionService.UpdatePavilionStatus(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePavilion([FromRoute] Guid eventId, [FromRoute] Guid id)
    {
        var response = await _pavilionService.DeletePavilion(eventId, id, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("check-pavilion-sessions-exists")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<bool>), StatusCodes.Status200OK)]
    public async Task<ActionResult> CheckPavilionSessionsExists([FromRoute] Guid eventId, CancellationToken token)
    {
        var response = await _pavilionService.CheckPavilionSessionsExist(eventId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
