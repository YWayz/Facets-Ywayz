using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberDocumentDeleteSpec : Specification<TeamMember>
{
    public TeamMemberDocumentDeleteSpec(Guid teamMemberId, Guid documentId)
    {
        Query.Where(w => w.Id == teamMemberId)
            .Include(i => i.Attachments.Where(w => w.Id == documentId));
    }
}
