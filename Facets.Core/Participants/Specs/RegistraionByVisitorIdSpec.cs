using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using static Facets.SharedKernal.AppConstants;

namespace Facets.Core.Participants.Specs;

internal sealed class RegistraionByVisitorIdSpec : Specification<VisitorRegistration, RegisteredVisitorDetailDto>
{
    public RegistraionByVisitorIdSpec(Guid eventID, Guid visitorId)
    {
        Query.Where(w => w.EventId == eventID && w.VisitorId == visitorId && w.RegistrationCancelled == false);

        Query.Select(s => new RegisteredVisitorDetailDto(s.Id,
                                                         s.VisitorId,
                                                         s.PassCategoryId,
                                                         s.VisitorAttendanceSchedules.Select(e => new VisitorAttendanceScheduleDto(e.Id,
                                                                                                                                   e.EventDateId,
                                                                                                                                   e.IsInvoiced,
                                                                                                                                   e.Cancelled))
                                                                                                                                   .ToList(),
                                                         s.RegisteredToEventOnsite,
                                                         s.EventId,
                                                         s.VisitorRegistrationCounterId,
                                                         s.PassCategory.PassCategorySettings.First(f => f.PassCategoryId == s.PassCategoryId)!.RateType,
                                                         s.PassCategory.VisitorPassCategoryType,
                                                         s.VisitorPavilionSessionAttendanceSchedules.Select(e => new VisitorPavilionSessionAttendanceScheduleDto(e.Id,
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
                                                                                                     .ToList()));
    }
}
