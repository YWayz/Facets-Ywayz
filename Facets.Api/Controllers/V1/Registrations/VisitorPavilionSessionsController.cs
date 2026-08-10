using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Registrations;

[Route("api/registrations/{visitorRegistrationId}/pavilion-sessions")]
public sealed class VisitorPavilionSessionsController : AdminAppControllerBase
{
    private readonly IVisitorPavilionService _visitorPavilionService;

    public VisitorPavilionSessionsController(IVisitorPavilionService visitorPavilionService)
    {
        _visitorPavilionService = visitorPavilionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorPavilionSessionAttendanceScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitorPavilionSessions([FromRoute] Guid visitorRegistrationId, CancellationToken token)
    {
        var response = await _visitorPavilionService.GetVisitorPavilionSessions(visitorRegistrationId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
