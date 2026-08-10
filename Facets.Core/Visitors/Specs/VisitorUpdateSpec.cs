using Ardalis.Specification;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorUpdateSpec : Specification<Visitor>
{
    public VisitorUpdateSpec(Guid visitorId)
    {
        Query.Where(w => w.Id == visitorId)
             .Include(w => w.Address);
    }
}
