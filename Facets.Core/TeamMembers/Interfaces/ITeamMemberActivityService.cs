using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberActivityService
{
    void AddTeamMemberActivity(TeamMemberActivity teamMemberActivity);
    Task<ResponseResult<IReadOnlyList<TeamMemberActivityDto>>> GetTeamMemberActivitiesByEvent(Paginator paginator, Guid teamMemberId, Guid eventId, CancellationToken token);
}
