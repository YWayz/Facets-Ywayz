using Facets.Core.Passes.DTOs;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberEventService
{
    Task<ResponseResult<IReadOnlyList<TeamMemberEventDto>>> GetTeamMembersByEvent(Paginator paginator, TeamMemberEventFilter filter, CancellationToken token);
    Task<ResponseResult<TeamMemberPassTemplateDto>> GetTeamMemberPassGenerationTemplate(Guid teamMemberId, CancellationToken token);
    Task<ResponseResult> RemoveTeamMemberFromEvent(Guid eventId, Guid teamMemberId, CancellationToken cancellationToken);
    Task<ResponseResult<QRVerifiedTeamMemberDto>> VerifyPass(TeamMemberPassVerificationDto model, CancellationToken token);
}
