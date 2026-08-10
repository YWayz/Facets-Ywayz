using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Filters;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Specs;

internal sealed class EventVisitorFilterSpec : Specification<VisitorAttendanceSchedule, Attendee>
{
    public EventVisitorFilterSpec(Guid eventDateId, Guid eventId, AttendeesFilter filter, bool isPayLater, OnSitePayingMode onSitePayingMode)
    {
        Query.Where(vts => vts.VisitorRegistration.EventId == eventId &&
                           vts.EventDateId == eventDateId &&
                           vts.Cancelled == false &&
                           vts.VisitorRegistration.Visitor.VisitorStatus == VisitorStatus.Active);

        if (filter.IsPrinted is false) Query.Where(w => w.IsPrinted == filter.IsPrinted);

        if (filter.AttendanceSheduleId.HasValue) Query.Where(w => w.Id == filter.AttendanceSheduleId.Value);

        if (isPayLater is false && onSitePayingMode == OnSitePayingMode.PayAtRegistration) Query.Where(w => w.IsInvoiced == true);

        if (string.IsNullOrWhiteSpace(filter.SearchTerm?.Trim()) is false)
        {
            string? searchTerm = filter.SearchTerm?.Trim();

            Query.Where(w => EF.Functions.Like(w.VisitorRegistration.Visitor.FirstName, searchTerm + "%") ||
                             EF.Functions.Like(w.VisitorRegistration.Visitor.LastName, searchTerm + "%") ||
                             EF.Functions.Like(w.VisitorRegistration.Visitor.NICNumber!, searchTerm + "%") ||
                             EF.Functions.Like(w.VisitorRegistration.Visitor.PassportNumber!, filter.SearchTerm + "%") ||
                             EF.Functions.Like(w.VisitorRegistration.Visitor.MobileNumber, searchTerm + "%"));
        }

        Query.Select(s => new Attendee
        (
            s.VisitorRegistration.Visitor.Id,
            s.VisitorRegistration.Visitor.FirstName,
            s.VisitorRegistration.Visitor.LastName,
            s.VisitorRegistration.Visitor.VisitorIdentityType,
            s.VisitorRegistration.Visitor.NICNumber,
            s.VisitorRegistration.Visitor.PassportNumber,
            s.VisitorRegistration.Visitor.MobileNumber,
            s.VisitorRegistration.PassCategoryId,
            s.VisitorRegistration.PassCategory.Name,
            s.Id,
            s.VisitorRegistrationId,
            s.VisitorRegistration.PassCategory.VisitorPassCategoryType,
            s.IsInvoiced && s.VisitorRegistration.VisitorPavilionSessionAttendanceSchedules.Where(w => w.Cancelled == false).All(a => a.IsInvoiced),
            s.VisitorRegistration.Event.OnSitePayingMode
        ));
    }
}
