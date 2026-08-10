using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberByIdSpec : Specification<TeamMember>
{
    public TeamMemberByIdSpec(Guid teamMemberId)
    {
        Query.Where(s => s.Id == teamMemberId);
    }
}
