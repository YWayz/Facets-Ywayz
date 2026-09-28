using Facets.Api.Policies;
using Facets.Api.Services;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Registrations;

[Route("api/registrations/{visitorRegistrationId}/pavilion-sessions")]
[AllowPublicSiteUser] // the public site reads a visitor's own booked sessions; ownership is checked below
public sealed class VisitorPavilionSessionsController : AdminAppControllerBase
{
    private readonly IVisitorPavilionService _visitorPavilionService;
    private readonly IPublicSiteOwnership _ownership;

    public VisitorPavilionSessionsController(IVisitorPavilionService visitorPavilionService, IPublicSiteOwnership ownership)
    {
        _visitorPavilionService = visitorPavilionService;
        _ownership = ownership;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<VisitorPavilionSessionAttendanceScheduleDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetVisitorPavilionSessions([FromRoute] Guid visitorRegistrationId, CancellationToken token)
    {
        if (_ownership.IsPublicSiteUser && await _ownership.OwnsRegistration(visitorRegistrationId, token) is false)
            return NotFound();

        var response = await _visitorPavilionService.GetVisitorPavilionSessions(visitorRegistrationId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
