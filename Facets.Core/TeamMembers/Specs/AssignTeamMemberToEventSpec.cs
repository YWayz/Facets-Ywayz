using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class AssignTeamMemberToEventSpec : Specification<TeamMember>
{
    public AssignTeamMemberToEventSpec(Guid teamMemberId, Guid eventId, Guid passCategoryId)
    {
        Query.Where(w => w.Id == teamMemberId)
             .Include(w => w.TeamMemberEvents.Where(s => s.EventId == eventId));
    }
}
