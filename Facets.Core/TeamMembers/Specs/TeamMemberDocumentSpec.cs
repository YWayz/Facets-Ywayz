using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberDocumentSpec : Specification<TeamMember>
{
    public TeamMemberDocumentSpec(Guid teamMemberId)
    {
        Query.Where(s => s.Id == teamMemberId)
             .Include(s => s.Attachments);
    }
}
