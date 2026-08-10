using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.Specs;

internal sealed class PassTemplateVisitorByVisitorIdSpec : Specification<VisitorAttendanceSchedule, PassTemplateVisitorDto>
{
    public PassTemplateVisitorByVisitorIdSpec(Guid visitorId, Guid eventDateId, Guid eventId)
    {
        Query.Where(vts => vts.VisitorRegistration.VisitorId == visitorId &&
                           vts.VisitorRegistration.EventId == eventId &&
                           vts.EventDateId == eventDateId &&
                           vts.VisitorRegistration.RegistrationCancelled == false);

        string currentDateTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        Query.Select(s => new PassTemplateVisitorDto
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
            currentDateTime,
            s.EventDate.Date,
            s.VisitorRegistration.Visitor.Documents.Where(w => w.IsDeleted == false)
                                                   .OrderByDescending(s => s.CreatedOn)
                                                   .First(p => p.VisitorId == visitorId && p.AttachmentType == AttachmentType.ProfileImage)
                                                   .AttachmentURL,
            s.VisitorRegistration.Event.Name,
            s.VisitorRegistration.PassCategory.PassCategorySettings.First(p => p.PassCategoryId == s.VisitorRegistration.PassCategoryId).Rate,
            s.VisitorRegistration.Visitor.Country.Name,
            s.VisitorRegistration.Visitor.CompanyName,
            s.VisitorRegistration.PassCategory.Color
            ));
    }
}