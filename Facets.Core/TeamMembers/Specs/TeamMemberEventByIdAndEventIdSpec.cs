using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberEventByIdAndEventIdSpec : Specification<TeamMemberEvent>
{
    public TeamMemberEventByIdAndEventIdSpec(Guid teamMemberId, Guid eventId)
    {
        Query.Where(s=>s.TeamMemberId == teamMemberId && s.EventId == eventId);
    }
}
