using Ardalis.Specification;
using Facets.Core.Common.Dtos;
using Facets.Core.Common.Filters;
using Facets.Core.Counters.Entities;

namespace Facets.Core.Common.Specs;

public sealed class RegistrationCounterLookupListSpec : Specification<VisitorRegistrationCounter, RegistrationCounterLookUpDto>
{
    public RegistrationCounterLookupListSpec(Guid eventId, RegistrationCounterLookupFilter filter)
    {
        Query.Where(w => w.EventId == eventId).OrderBy(x => x.Name);

        if (filter.IsLocked.HasValue) Query.Where(w => w.IsLocked == filter.IsLocked.Value);

        if (filter.CounterTypes is not null) Query.Where(w => filter.CounterTypes.Contains(w.CounterType));

        Query.Select(e => new RegistrationCounterLookUpDto
        (
         e.Id,
         e.Name,
         e.Description,
         e.IsLocked,
         e.EventId, 
         e.CounterType
        ));
    }
}
