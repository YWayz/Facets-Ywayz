using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class GetPavilionToAddSessionsSpec : Specification<Pavilion>
{
    public GetPavilionToAddSessionsSpec(Guid eventId, Guid id)
    {
        Query.Where(w => w.EventId == eventId && w.Id == id);
    }
}
