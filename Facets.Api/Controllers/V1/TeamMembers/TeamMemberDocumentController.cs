using Facets.Core.Common.Dtos;
using Facets.Core.Security.AuthPolicies;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Api.Controllers.V1.TeamMembers;

[Route("api/team-members/{teamMemberId}/documents")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public class TeamMemberDocumentController : AdminAppControllerBase
{
    private readonly ITeamMemberService _teamMemberService;
    public TeamMemberDocumentController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.Register)]
    [ProducesResponseType(typeof(ResponseResult<List<FileDto>>), StatusCodes.Status201Created)]
    public async Task<ActionResult> UploadTeamMemberDocuments([FromRoute] Guid teamMemberId, [FromForm] List<KeyValuePair<AttachmentType, IFormFile>> files)
    {
        var response = await _teamMemberService.UploadTeamMemberDocuments(teamMemberId, files, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.ViewAttachments)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyCollection<FileDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetTeamMemeberDocuments([FromRoute] Guid teamMemberId, CancellationToken cancellationToken)
    {
        var response = await _teamMemberService.GetTeamMemberDocuments(teamMemberId, cancellationToken);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpDelete("{documentId}")]
    [Authorize(policy: ApplicationAuthPolicy.TeamMemberPolicy.CancelTeamMemberRegistration)]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteTeamMemberDocument([FromRoute] Guid teamMemberId, [FromRoute] Guid documentId)
    {
        var response = await _teamMemberService.DeleteTeamMemberDocument(teamMemberId, documentId, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
