using Facets.Core.Payments.Events;
using MediatR;

namespace Facets.Core.Payments.EventHandlers;

internal sealed class MarkAttendanceSchedulesEventHandler : INotificationHandler<VisitorInvoicingEventEvent>
{
    public  Task Handle(VisitorInvoicingEventEvent notification, CancellationToken cancellationToken)
    {
        foreach (var visitorEventDateAttendanceSchedule in notification.Invoice.VisitorRegistration.VisitorAttendanceSchedules)
        {
            visitorEventDateAttendanceSchedule.MarkAsCanAttendEvent();
        }

        foreach (var visitorPavilionSessionAttendanceSchedule in notification.Invoice.VisitorRegistration.VisitorPavilionSessionAttendanceSchedules)
        {
            visitorPavilionSessionAttendanceSchedule.MarkAsCanAttendPavilionSession();
        }

        return Task.CompletedTask;
    }
}
