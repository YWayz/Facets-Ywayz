using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class EventPaymentSettingsSpec: Specification<Event, PaymentSettingsDto>
{
    public EventPaymentSettingsSpec(Guid eventId)
    {
        Query.Where(e => e.Id == eventId);

        Query.Select(s => new PaymentSettingsDto(s.Id, s.OnSitePayingMode, s.PayLaterForOnlineRegistration));        
    }
}
