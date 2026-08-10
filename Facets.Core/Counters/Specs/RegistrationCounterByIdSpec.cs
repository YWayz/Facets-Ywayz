using Ardalis.Specification;
using Facets.Core.Counters.DTOs;
using Facets.Core.Counters.Entities;

namespace Facets.Core.Counters.Specs;

public sealed class RegistrationCounterByIdSpec : Specification<VisitorRegistrationCounter, RegistrationCounterDto>
{
    public RegistrationCounterByIdSpec(Guid eventId, Guid registrationCounterId)
    {
        Query.Where(w => w.Id == registrationCounterId && w.EventId == eventId);

        Query.Select(e => new RegistrationCounterDto
        (
         e.Id,
         e.Name,
         e.Description,
         e.IsLocked,
         e.EventId,
         e.UserAssignedRegistrationCounters.Select(s => s.AssginedUser.FirstName + ' ' + s.AssginedUser.LastName).FirstOrDefault(),
         e.CounterType
         ));
    }
}
