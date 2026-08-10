using Facets.Core.Passes.DTOs;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.TeamMembers;

[Route("api/team-member-passes")]
public class TeamMemberPassController : AdminAppControllerBase
{
    private readonly ITeamMemberEventService _teamMemberEventService;

    public TeamMemberPassController(ITeamMemberEventService teamMemberEventService)
    {
        _teamMemberEventService = teamMemberEventService;
    }

    [HttpGet("template/{teamMemberId}")]
    [Authorize(policy: ApplicationAuthPolicy.PassGenerationPolicy.TeamMemberPassGeneration)]
    [ProducesResponseType(typeof(ResponseResult<TeamMemberPassTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMemberGenerationTemplate( [FromRoute] Guid teamMemberId, CancellationToken token)
    {
        var response = await _teamMemberEventService.GetTeamMemberPassGenerationTemplate(teamMemberId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("verify-qr")]
    [ProducesResponseType(typeof(ResponseResult<QRVerifiedTeamMemberDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> veryifyByQR([FromBody] TeamMemberPassVerificationDto model)
    {
        var response = await _teamMemberEventService.VerifyPass(model, CancellationToken.None);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
