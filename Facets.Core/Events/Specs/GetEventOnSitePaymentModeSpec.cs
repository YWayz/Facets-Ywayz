using Ardalis.Specification;
using Facets.Core.Events.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Specs;

internal sealed class GetEventOnSitePaymentModeSpec: Specification<Event, OnSitePayingMode>
{
    public GetEventOnSitePaymentModeSpec(Guid eventId)
    {
        Query.Where(w => w.Id == eventId);

        Query.Select(s => s.OnSitePayingMode);
    }
}
