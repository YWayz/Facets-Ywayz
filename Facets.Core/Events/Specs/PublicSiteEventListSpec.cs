using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Specs;

internal sealed class PublicSiteEventListSpec : Specification<Event, PublicSiteEventSummaryDto>
{
    DateTimeOffset currentDateTime = DateTimeOffset.UtcNow;

    public PublicSiteEventListSpec()
    {
        Query.Where(w => w.IsDeleted == false &&
                         w.PublishedToPublicSite == true &&
                         w.Status == EventStatus.Active &&
                         w.VisitorRegistrationEndsOn.Date >= currentDateTime.Date)
             .OrderBy(s => s.VisitorRegistrationStartsOn);

        Query.Select(e => new PublicSiteEventSummaryDto
        (
         e.Id,
         e.Name,
         e.Description,
         e.LogoUrl,
         e.VisitorRegistrationStartsOn,
         e.VisitorRegistrationEndsOn,
         e.EventDates.OrderBy(o => o.Date).Select(s => s.Date).ToList(),
         e.VisitorRegistrationStartsOn.Date > currentDateTime.Date
        ));
    }
}
