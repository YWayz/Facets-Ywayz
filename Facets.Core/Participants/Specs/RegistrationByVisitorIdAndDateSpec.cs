using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class RegistrationByVisitorIdAndDateSpec : Specification<VisitorRegistration, VisitorAttendanceScheduleDto?>
{
    public RegistrationByVisitorIdAndDateSpec(Guid eventId, Guid visitorId, DateTimeOffset currentDate)
    {
        Query.Where(w => w.EventId == eventId && w.VisitorId == visitorId && w.RegistrationCancelled == false);

        Query.Select(s => s.VisitorAttendanceSchedules.Where(w => w.EventDate.Date.Date == currentDate.Date)
                                                      .Select(e => new VisitorAttendanceScheduleDto(e.Id,
                                                                                                    e.EventDateId,
                                                                                                    e.IsInvoiced,
                                                                                                    e.Cancelled))
                                                      .FirstOrDefault());
    }
}
