using Ardalis.Specification;
using Facets.Core.Counters.Entities;

namespace Facets.Core.Counters.Specs;

internal sealed class RegistrationCounterForAssignmentSpec : Specification<VisitorRegistrationCounter>
{
    public RegistrationCounterForAssignmentSpec(Guid eventId, Guid counterId)
    {
        Query.Where(w => w.Id == counterId && w.EventId == eventId);
    }
}
