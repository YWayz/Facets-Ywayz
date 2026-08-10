using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.Core.TeamMembers.Specs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.TeamMembers.Services;

public sealed class TeamMemberActivityService: ITeamMemberActivityService
{
    private readonly ITeamMemberActivityRepository _teamMemberActivityRepository;

    public TeamMemberActivityService(ITeamMemberActivityRepository teamMemberActivityRepository)
    {
        _teamMemberActivityRepository = teamMemberActivityRepository;
    }

    public void AddTeamMemberActivity(TeamMemberActivity teamMemberActivity)
    {
        _teamMemberActivityRepository.AddTeamMemberActivity(teamMemberActivity);
    }

    public async Task<ResponseResult<IReadOnlyList<TeamMemberActivityDto>>> GetTeamMemberActivitiesByEvent(Paginator paginator, Guid teamMemberId, Guid eventId, CancellationToken token)
    {
        var (list, totalRecords) = await _teamMemberActivityRepository.GetProjectedListBySpec(paginator, new TeamMemberActivitiesByEventSpec(eventId, teamMemberId), token);

        return new(list, totalRecords);

    }

}
