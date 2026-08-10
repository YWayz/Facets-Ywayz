using Ardalis.Specification;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorBlacklistSpec : Specification<Visitor>
{
    public VisitorBlacklistSpec(Guid visitorId)
    {
        Query.Where(w => w.Id == visitorId && w.IsDeleted == false);
    }
}
