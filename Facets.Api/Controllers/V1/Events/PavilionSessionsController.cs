using Facets.Core.Events.DTOs;
using Facets.Core.Events.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Events;

[Route("api/events/{eventId}/pavilions/{pavilionId}/pavilion-sessions")]
public sealed class PavilionSessionsController : AdminAppControllerBase
{
    private readonly IPavilionService _pavilionService;

    public PavilionSessionsController(IPavilionService pavilionService)
    {
        _pavilionService = pavilionService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.CreatePavilionSession)]
    [ProducesResponseType(typeof(ResponseResult<PavilionSessionDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePavilionSession([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, [FromBody] CreateOrUpdatePavilionSessionItemDto model)
    {
        var response = await _pavilionService.CreatePavilionSession(eventId, pavilionId, model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetPavilionSessions), new { eventId = eventId, pavilionId = pavilionId}, response) : UnsuccessfullResponse(response);
    }

    [HttpGet(Name = nameof(GetPavilionSessions))]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.ViewPavilionSession)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilionSessions([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, CancellationToken token)
    {
        var response = await _pavilionService.GetPavilionSessions(eventId, pavilionId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{pavilionSessionId}")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.EditPavilionSession)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePavilionSessions([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, [FromRoute] Guid pavilionSessionId, CreateOrUpdatePavilionSessionItemDto model)
    {
        var response = await _pavilionService.UpdatePavilionSession(eventId, pavilionId, pavilionSessionId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{pavilionSessionId}")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.SessionDelete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePavilion([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, [FromRoute] Guid pavilionSessionId)
    {
        var response = await _pavilionService.DeletePavilionSession(eventId, pavilionSessionId, pavilionId, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
