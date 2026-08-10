using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class IsEventMarkedAsPayLaterForOnlineRegistrationSpec : Specification<Event, bool>
{
    public IsEventMarkedAsPayLaterForOnlineRegistrationSpec(Guid eventId)
    {
        Query.Where(w => w.Id == eventId);

        Query.Select(s => s.PayLaterForOnlineRegistration);
    }
}
