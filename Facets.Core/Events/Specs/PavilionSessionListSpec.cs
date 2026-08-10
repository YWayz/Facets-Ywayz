using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionSessionListSpec : Specification<Pavilion, PavilionSessionSummaryDto>
{
    public PavilionSessionListSpec(Guid eventId, Guid pavilionId)
    {
        Query.Where(w => w.EventId == eventId && w.Id == pavilionId);

        Query.SelectMany(s => s.PavilionSessions.OrderByDescending(aw => aw.CreatedOn)
                                                .Select(e => new PavilionSessionSummaryDto(e.Id,
                                                                                           e.EventDateId,
                                                                                           e.StartTime,
                                                                                           e.EndTime,
                                                                                           e.AllowedVisitorCount,
                                                                                           e.PavilionId,
                                                                                           e.EventDate.Date,
                                                                                           e.VisitorPavilionSessionAttendanceSchedules.Any()
                                                                                           )));
    }
}