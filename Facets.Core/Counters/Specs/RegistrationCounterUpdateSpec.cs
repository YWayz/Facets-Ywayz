using Ardalis.Specification;
using Facets.Core.Counters.Entities;
using Microsoft.Extensions.Logging;

namespace Facets.Core.Counters.Specs;

public sealed class RegistrationCounterUpdateSpec : Specification<VisitorRegistrationCounter>
{
    public RegistrationCounterUpdateSpec(Guid eventId, Guid id)
    {
        Query.Where(e => e.Id == id && e.EventId == eventId);
    }
}
