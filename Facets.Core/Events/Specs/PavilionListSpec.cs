using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;
using Facets.Core.Events.Filters;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionListSpec : Specification<Pavilion, PavilionSummaryDto>
{
    public PavilionListSpec(Guid eventId, PavilionFilter filter)
    {
        Query.Where(w => w.EventId == eventId).OrderByDescending(s => s.CreatedOn);

        if (filter.PavilionStatus.HasValue) Query.Where(w => w.Status == filter.PavilionStatus.Value);

        Query.Select(e => new PavilionSummaryDto
        (
         e.Id,
         e.Name,
         e.PavilionSessions.Where(w => w.PavilionId == e.Id).Count(),
         e.Status,
         e.PavilionSessions.Where(w => w.PavilionId == e.Id).Select(s => new PavilionSessionSummaryDto(s.Id,
                                                                                                       s.EventDateId,
                                                                                                       s.StartTime,
                                                                                                       s.EndTime,
                                                                                                       s.AllowedVisitorCount,
                                                                                                       s.PavilionId,
                                                                                                       s.EventDate.Date,
                                                                                                       s.VisitorPavilionSessionAttendanceSchedules.Any(a=>a.Cancelled == false)
                                                                                                       )).ToList()
        ));
    }
}
