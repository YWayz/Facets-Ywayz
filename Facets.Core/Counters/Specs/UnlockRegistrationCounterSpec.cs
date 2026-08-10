using Ardalis.Specification;
using Facets.Core.Counters.Entities;

namespace Facets.Core.Counters.Specs;

internal sealed class UnlockRegistrationCounterSpec : Specification<VisitorRegistrationCounter>
{
    public UnlockRegistrationCounterSpec(Guid eventId, Guid counterId)
    {
        Query.Where(w => w.EventId == eventId &&
                         w.Id == counterId)
             .Include(w => w.UserAssignedRegistrationCounters.Where(w => w.UnAssignedAt == null && w.UnAssignedByUserId == null));
    }
}
