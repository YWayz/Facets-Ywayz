using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionUpdateSpec : Specification<Pavilion>
{
    public PavilionUpdateSpec(Guid eventId, Guid pavilionId)
    {
        Query.Where(e => e.Id == pavilionId && e.EventId == eventId);
    }
}
