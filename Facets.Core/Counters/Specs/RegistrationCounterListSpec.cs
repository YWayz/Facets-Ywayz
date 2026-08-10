using Ardalis.Specification;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Entities;
using Facets.Core.Counters.Filters;

namespace Facets.Core.Counters.Specs;

public sealed class RegistrationCounterListSpec : Specification<VisitorRegistrationCounter, RegistrationCounterDto>
{
    public RegistrationCounterListSpec(Guid eventId, RegistrationCounterFilter filter)
    {
        Query.Where(w => w.EventId == eventId).OrderByDescending(s => s.CreatedOn);

        if (filter.IsLocked.HasValue) Query.Where(w => w.IsLocked == filter.IsLocked.Value);

        Query.Select(e => new RegistrationCounterDto
        (
         e.Id,
         e.Name,
         e.Description,
         e.IsLocked,
         e.EventId,
         e.UserAssignedRegistrationCounters.OrderByDescending(d => d.CreatedOn).Select(s => s.AssginedUser.FullName).FirstOrDefault(),
         e.CounterType
        ));
    }
}
