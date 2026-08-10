using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Events.Specs;

internal sealed class EventDateByDateSpec : Specification<Event, EventDateDetailDto?>
{
    public EventDateByDateSpec(Guid eventId, DateTimeOffset date)
    {
        Query.Where(w => w.Id == eventId && w.Status == EventStatus.Active && w.IsDeleted == false);

        Query.Select(w => w.EventDates.Where(a => a.Date.Date == date.Date)
                                      .Select(s => new EventDateDetailDto(s.EventId,
                                                                          s.Id,
                                                                          s.Date)).FirstOrDefault());
    }
}
