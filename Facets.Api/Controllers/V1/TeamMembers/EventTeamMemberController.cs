using Facets.Core.Security.AuthPolicies;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Filters;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.TeamMembers;

[Route("api/event-team-members")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class EventTeamMemberController : AdminAppControllerBase
{
    private readonly ITeamMemberService _teamMemberService;
    private readonly ITeamMemberEventService _teamMemberEventService;

    public EventTeamMemberController(ITeamMemberService teamMemberService, ITeamMemberEventService teamMemberEventService)
    {
        _teamMemberService = teamMemberService;
        _teamMemberEventService = teamMemberEventService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status201Created)]
    public async Task<ActionResult> AssignTeamMemberToEvents([FromBody] AssignTeamMemberToEventsDto assignTeamMemberToEventsDto)
    {
        var response = await _teamMemberService.AssignTeamMemberToEvent(assignTeamMemberToEventsDto.teamMemberId, assignTeamMemberToEventsDto.eventId, assignTeamMemberToEventsDto.passCategoryId, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<TeamMemberEventDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMembersByEvent([FromQuery] Paginator paginator, [FromQuery] TeamMemberEventFilter filter, CancellationToken token)
    {
        var response = await _teamMemberEventService.GetTeamMembersByEvent(paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpDelete("{eventId}/team-members/{teamMemberId}")]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.CancelTeamMemberRegistration)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveTeamMemberFromEvent([FromRoute] Guid eventId, [FromRoute] Guid teamMemberId)
    {
        var response = await _teamMemberEventService.RemoveTeamMemberFromEvent(eventId, teamMemberId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
