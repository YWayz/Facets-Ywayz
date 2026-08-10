using Ardalis.Specification;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Filters;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Specs;

internal sealed class VisitorInvoiceFilterSpec : Specification<Invoice, InvoicePaymentDto>
{
    public VisitorInvoiceFilterSpec(Guid eventId, Guid visitorId, InvoiceFilter filter)
    {
        Query.Where(w => w.EventId == eventId && w.VisitorId == visitorId && w.InvoiceCancelled == false);

        if (filter.EventDateIds is not null)
            Query.Where(w => w.VisitorRegistration.VisitorAttendanceSchedules.Any(a => filter.EventDateIds.Contains(a.EventDateId)));

        if (filter.PaymentStatuses is not null)
            Query.Where(w => filter.PaymentStatuses.Contains(w.PaymentStatus));

        Query.Select(s => new InvoicePaymentDto(s.RateType == RateType.PerDayRate ? s.InvoiceLineItems
                                                                                     .Where(i => s.VisitorRegistration
                                                                                                  .VisitorAttendanceSchedules
                                                                                                  .Where(v => filter.EventDateIds!.Contains(v.EventDateId))
                                                                                                  .Any(ss => ss.Id == i.ItemId))
                                                                                     .Select(a => a.Amount).FirstOrDefault()



                                                                                     +

                                                                                     s.InvoiceLineItems
                                                                                     .Where(i => s.VisitorRegistration
                                                                                                  .VisitorPavilionSessionAttendanceSchedules
                                                                                                  .Where(v => filter.EventDateIds!.Contains(v.PavilionSession.EventDateId))
                                                                                                  .Any(ss => ss.Id == i.ItemId))
                                                                                     .Select(a => a.Amount).Sum()

                                                                                     : s.TotalAmount,
                                                s.Payments.Select(a => a.PaymentMethod).FirstOrDefault(),
                                                s.RateType,
                                                s.PaymentStatus));
    }
}
