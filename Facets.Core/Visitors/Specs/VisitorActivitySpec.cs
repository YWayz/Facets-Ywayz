using Ardalis.Specification;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorActivitySpec : Specification<VisitorActivity, VisitorActivityDto>
{
    public VisitorActivitySpec(Guid visitorId, Guid eventId)
    {
        Query.Where(w => w.VisitorId == visitorId && w.FacetsEventId == eventId);

        Query.Select(s => new VisitorActivityDto(
            s.Id,
            s.CreatedOn,
            s.Description,
            s.VisitorId,
            s.FacetsEventId,
            s.VisitorActivityType
        ));
    }
}
