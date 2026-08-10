using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class EventPaymentSettingsUpdateSpec: Specification<Event>
{
    public EventPaymentSettingsUpdateSpec(Guid eventId)
    {
        Query.Where(s => s.Id == eventId);
    }
}
