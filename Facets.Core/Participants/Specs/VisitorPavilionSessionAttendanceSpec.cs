using Ardalis.Specification;
using Facets.Core.Events.Entities;
using Facets.Core.Participants.DTOs;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Core.Participants.Specs;

internal sealed class VisitorPavilionSessionAttendanceSpec : Specification<PavilionSession, VisitorPavilionSessionAttendanceScheduleDto>
{
    public VisitorPavilionSessionAttendanceSpec(Guid visitorRegistrationId)
    {
        Query.SelectMany(s => s.VisitorPavilionSessionAttendanceSchedules
                               .Where(w => w.VisitorRegistrationId == visitorRegistrationId)
                               .Select(c => new VisitorPavilionSessionAttendanceScheduleDto(c.Id,
                                                                                            s.EventDateId,
                                                                                            s.PavilionId,
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
                                                                                            c.VisitorAttendedAt
                                                                                            )).ToList());
    }
}
