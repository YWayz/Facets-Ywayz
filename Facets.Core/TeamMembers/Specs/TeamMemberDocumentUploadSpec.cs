using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberDocumentUploadSpec : Specification<TeamMember>
{
    public TeamMemberDocumentUploadSpec(Guid teamMemberId)
    {
        Query.Where(w => w.Id == teamMemberId)
            .Include(i => i.Attachments);
    }
}
