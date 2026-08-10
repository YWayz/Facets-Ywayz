using Facets.Core.Participants.Events;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using MediatR;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.EventHandlers;

internal sealed class VisitorCancellingFromRegsiteredEventDayActivityHandler : INotificationHandler<VisitorRegsiteredAttendanceDatesCancellingEvent>
{
    private readonly IVisitorService _visitorService;

    public VisitorCancellingFromRegsiteredEventDayActivityHandler(IVisitorService visitorService)
    {
        _visitorService = visitorService;
    }

    public Task Handle(VisitorRegsiteredAttendanceDatesCancellingEvent notification, CancellationToken cancellationToken)
    {
        var attendanceSheduledEventDates = notification.AttendanceShedulesToCancel.Where(w => w.Cancelled is true)
                                                                                  .Select(s => s.EventDate.Date)
                                                                                  .OrderBy(o => o);

        var localDates = attendanceSheduledEventDates.Select(s => s.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat()).ToList();

        string commaDelimeteredDates = string.Join(", ", localDates);

        var registrationCancelledDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        string description = $"{registrationCancelledDateAndTime} - Cancelled registration for date(s): {commaDelimeteredDates}";

        _visitorService.AddVisitorActivity(new VisitorActivity(notification.VisitorRegistration.EventId,
                                                               notification.VisitorRegistration.VisitorId,
                                                               description,
                                                               VisitorActivityType.RegistrationCancelled));

        return Task.CompletedTask;
    }
}
