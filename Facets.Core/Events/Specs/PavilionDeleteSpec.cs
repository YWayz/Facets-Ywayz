using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionDeleteSpec : Specification<Pavilion>
{
    public PavilionDeleteSpec(Guid eventId, Guid id)
    {
        Query.Where(e => e.EventId == eventId && e.Id == id);
    }
}
