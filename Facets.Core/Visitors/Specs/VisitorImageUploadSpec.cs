using Ardalis.Specification;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorImageUploadSpec : Specification<Visitor>
{
    public VisitorImageUploadSpec(Guid visitorId)
    {
        Query.Where(w => w.Id == visitorId);
    }
}
