using Facets.Core.Events.Interfaces;
using Facets.Core.Payments.Events;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using MediatR;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.EventHandlers;

internal sealed class VisitorRegisteringEventDatesEventHandler : INotificationHandler<VisitorInvoicingEventEvent>
{
    private readonly IVisitorService _visitorService;
    private readonly IEventService _eventService;

    public VisitorRegisteringEventDatesEventHandler(IVisitorService visitorService, IEventService eventService)
    {
        _visitorService = visitorService;
        _eventService = eventService;
    }

    public async Task Handle(VisitorInvoicingEventEvent notification, CancellationToken cancellationToken)
    {
        var eventRespone = await _eventService.GetEventById(notification.VisitorRegistration.EventId, cancellationToken);

        var assignedEventDateIds = notification.VisitorRegistration.VisitorAttendanceSchedules.Select(s => s.EventDateId).ToList();

        if (assignedEventDateIds.Count is 0) return;

        var eventDates = eventRespone.Data!.EventDates
                                           .Where(w => assignedEventDateIds.Contains(w.Key))
                                           .Select(s => s.Value)
                                           .OrderBy(o => o);

        var localDates = eventDates.Select(s => s.GetLocalTime(AppConstants.SriLankaTimeZone).ToApplicationDateFormat()).ToList();

        string commaDelimeteredDates = string.Join(", ", localDates);

        var registrationDateAndTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        string activity = $"{registrationDateAndTime} - Registered for date(s): {commaDelimeteredDates}";

        _visitorService.AddVisitorActivity(new VisitorActivity(notification.VisitorRegistration.EventId,
                                                               notification.VisitorRegistration.VisitorId,
                                                               activity,
                                                               VisitorActivityType.Registered));
    }
}
