using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Filters;
using System.Linq;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Core.Participants.Specs;

internal sealed class GetVisitorPassRegistrationByVisitorIdSpec : Specification<VisitorRegistration, VisitorPassRegistrationDto>
{
    public GetVisitorPassRegistrationByVisitorIdSpec(Guid visitorId, Guid visitorRegistrationId, Guid eventId, RegistrationFilter filter)
    {
        Query.Where(w => w.Id == visitorRegistrationId && w.VisitorId == visitorId && w.EventId == eventId);

        Query.Select(s => new VisitorPassRegistrationDto(
            s.VisitorAttendanceSchedules.Where(w => w.IsInvoiced == true && (filter.EventDateIds == null || filter.EventDateIds.Contains(w.EventDateId)))
                                        .OrderBy(s=>s.EventDate.Date)
                                        .Select(e => new VisitorAttendedScheduleDto(e.Id,
                                                                                    e.EventDate.Date,
                                                                                    e.VisitorAttended,
                                                                                    e.VisitorAttendedAt,
                                                                                    e.EventDate.Id,
                                                                                    e.Cancelled,
                                                                                    e.AttendanceScheduledOnsite ? RegistrationMethod.OnSite : RegistrationMethod.Online,
                                                                                    e.CreatedOn))
                                        .ToList(),
            s.PassCategory.Name,
            s.Id,
            s.VisitorPavilionSessionAttendanceSchedules.Where(w => w.IsInvoiced == true && (filter.EventDateIds == null || 
                                                                                            filter.EventDateIds.Contains(w.PavilionSession.EventDateId)))
                                                       .OrderBy(o => o.PavilionSession.EventDate.Date)
                                                       .Select(e => new VisitorPavilionSessionAttendanceScheduleDto(e.Id,
                                                                                                                    e.PavilionSession.EventDateId,
                                                                                                                    e.PavilionSession.PavilionId,
                                                                                                                    e.PavilionSessionId,
                                                                                                                    e.Cancelled,
                                                                                                                    e.IsInvoiced,
                                                                                                                    e.PavilionSession.Pavilion.Name,
                                                                                                                    e.PavilionSession.EventDate.Date,
                                                                                                                    e.PavilionSession.StartTime,
                                                                                                                    e.PavilionSession.EndTime,
                                                                                                                    e.VisitorRegistration.VisitorAttendanceSchedules
                                                                                                                    .FirstOrDefault(f => f.VisitorRegistrationId == e.VisitorRegistrationId)!
                                                                                                                    .AttendanceScheduledOnsite ? RegistrationMethod.OnSite : RegistrationMethod.Online,
                                                                                                                    e.CreatedOn,
                                                                                                                    e.VisitorAttendedAt
                                                                                                                    ))
                                                       .ToList()
        ));
    }
}
