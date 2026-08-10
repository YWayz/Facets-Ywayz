using Ardalis.Specification;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal class TeamMemberActivitiesByEventSpec : Specification<TeamMemberActivity, TeamMemberActivityDto>
{
    public TeamMemberActivitiesByEventSpec(Guid eventId, Guid teamMemberId)
    {
        Query.Where(s => s.TeamMemberId == teamMemberId && s.FacetsEventId == eventId);

        Query.Select(s => new TeamMemberActivityDto(
            s.Id,
            s.CreatedOn,
            s.Description,
            s.TeamMemberId,
            s.FacetsEventId,
            s.ActivityType
            ));
    }
}
