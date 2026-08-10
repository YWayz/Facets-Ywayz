using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Core.Participants.Specs;

internal sealed class PassRegistrationByVisitorIdAndDateSpec : Specification<VisitorRegistration, VisitorPavilionSessionAttendanceScheduleDto?>
{
    public PassRegistrationByVisitorIdAndDateSpec(Guid eventId, Guid visitorId, Guid pavilionId, Guid pavilionSessionId, DateTimeOffset currentDate)
    {
        Query.Where(w => w.EventId == eventId && w.VisitorId == visitorId && w.RegistrationCancelled == false);

        Query.Select(s => s.VisitorPavilionSessionAttendanceSchedules.Where(w => w.PavilionSession.PavilionId == pavilionId && 
                                                                                 w.PavilionSessionId == pavilionSessionId && 
                                                                                 w.PavilionSession.EventDate.Date.Date == currentDate.Date)
                                                                     .Select(c => new VisitorPavilionSessionAttendanceScheduleDto(c.Id,
                                                                                            c.PavilionSession.EventDateId,
                                                                                            c.PavilionSession.PavilionId,
                                                                                            c.PavilionSessionId,
                                                                                            c.Cancelled,
                                                                                            c.IsInvoiced,
                                                                                            c.PavilionSession.Pavilion.Name,
                                                                                            c.PavilionSession.EventDate.Date,
                                                                                            c.PavilionSession.StartTime,
                                                                                            c.PavilionSession.EndTime,
                                                                                            c.VisitorRegistration.VisitorAttendanceSchedules
                                                                                                                    .FirstOrDefault(f => f.VisitorRegistrationId == c.VisitorRegistrationId)!
                                                                                                                    .AttendanceScheduledOnsite ? RegistrationMethod.OnSite : RegistrationMethod.Online,
                                                                                            c.CreatedOn,
                                                                                            c.VisitorAttendedAt))
                                                      .FirstOrDefault());
    }
}
