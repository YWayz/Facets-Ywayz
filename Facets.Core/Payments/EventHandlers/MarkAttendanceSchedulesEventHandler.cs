using Facets.Core.Payments.Events;
using MediatR;

namespace Facets.Core.Payments.EventHandlers;

internal sealed class MarkAttendanceSchedulesEventHandler : INotificationHandler<VisitorInvoicingEventEvent>
{
    public  Task Handle(VisitorInvoicingEventEvent notification, CancellationToken cancellationToken)
    {
        // Only what this invoice billed becomes attendable. Marking every schedule on the registration let a
        // visitor invoice one day, add more days, pay the one-day invoice and attend all of them.
        var paidItemIds = notification.Invoice.InvoiceLineItems
                                              .Where(l => l.IsDeleted is false)
                                              .Select(l => l.ItemId)
                                              .ToHashSet();

        var registration = notification.Invoice.VisitorRegistration;

        foreach (var schedule in registration.VisitorAttendanceSchedules.Where(s => paidItemIds.Contains(s.Id)))
        {
            schedule.MarkAsCanAttendEvent();
        }

        foreach (var schedule in registration.VisitorPavilionSessionAttendanceSchedules.Where(s => paidItemIds.Contains(s.Id)))
        {
            schedule.MarkAsCanAttendPavilionSession();
        }

        return Task.CompletedTask;
    }
}
