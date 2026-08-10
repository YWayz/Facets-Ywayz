using Facets.Core.Common.Dtos;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.TeamMembers;

[Route("api/team-members")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class TeamMemberController : AdminAppControllerBase
{
    private readonly ITeamMemberService _teamMemberService;
    private readonly ITeamMemberActivityService _teamMemberActivityService;

    public TeamMemberController(ITeamMemberService teamMemberService, ITeamMemberActivityService teamMemberActivityService)
    {
        _teamMemberService = teamMemberService;
        _teamMemberActivityService = teamMemberActivityService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.Register)]
    [ProducesResponseType(typeof(ResponseResult<CreatedTeamMemberDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreateTeamMember([FromBody] CreateTeamMemberDto model)
    {
        var response = await _teamMemberService.CreateTeamMember(model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetTeamMemberById), new { id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetTeamMemberById))]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.View)]
    [ProducesResponseType(typeof(ResponseResult<TeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMemberById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _teamMemberService.GetTeamMemberById(id, cancellationToken);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ResponseResult<TeamMemberSearchDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> SearchMember([FromQuery] string? searchValue, CancellationToken token)
    {
        var response = await _teamMemberService.SearchMember(searchValue, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPost("{id}/profile-image")]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.Register)]
    [ProducesResponseType(typeof(ResponseResult<FileDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddTeamMemberProfileImage([FromRoute] Guid id, IFormFile file)
    {
        var response = await _teamMemberService.AddTeamMemberProfileImage(teamMemberId: id, file, CancellationToken.None);

        return response.Success ? Created(response.Data!.URI, response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.UpdateTeamMember)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateTeamMember([FromRoute] Guid id, [FromBody] UpdateTeamMemberDto model)
    {
        var response = await _teamMemberService.UpdateTeamMember(id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{teamMemberId}/profile-image")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTeamMemberProfileImage([FromRoute] Guid teamMemberId)
    {
        var response = await _teamMemberService.DeleteTeamMemberProfileImage(teamMemberId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpGet("{teamMemberId}/event/{eventId}/activities")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<TeamMemberActivityDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMembersByEvent([FromRoute] Guid teamMemberId, [FromRoute] Guid eventId, [FromQuery] Paginator paginator, CancellationToken token)
    {
        var response = await _teamMemberActivityService.GetTeamMemberActivitiesByEvent(paginator, teamMemberId, eventId, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }


    [HttpPut("{id}/mark-as-blacklisted")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.Blacklist)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MarkAsBlackListed([FromRoute] Guid id)
    {
        var response = await _teamMemberService.MarkAsBlackListed(id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }


    [HttpPut("{id}/remove-blacklisted")]
    [Authorize(policy: ApplicationAuthPolicy.VisitorPolicy.Blacklist)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveBlackListed([FromRoute] Guid id)
    {
        var response = await _teamMemberService.RemoveBlackListedMember(id, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
