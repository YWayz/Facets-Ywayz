using Ardalis.Specification;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Filters;

namespace Facets.Core.Payments.Specs;

internal sealed class GetInvoiceByRegistrationIdSpec : Specification<Invoice, InvoiceDto>
{
    public GetInvoiceByRegistrationIdSpec(Guid registrationId, Guid eventId, RegistrationInvoiceFilter filter)
    {
        Query.Where(w => w.VisitorRegistrationId == registrationId && w.EventId == eventId);

        if (filter.PaymentStatuses is not null) Query.Where(w => filter.PaymentStatuses.Contains(w.PaymentStatus));

        if (filter.InvoiceIds is not null) Query.Where(w => filter.InvoiceIds.Contains(w.Id));

        Query.Select(s => new InvoiceDto(s.VisitorRegistrationId,
                                         s.VisitorId,
                                         s.EventId,
                                         s.TotalAmount,
                                         s.InvoiceCancelled,
                                         s.InvoiceCancelledOn,
                                         s.InvoiceLineItems.Select(x => new InvoiceLineItemDto(x.CreatedOn,
                                                                                               x.InvoiceId,
                                                                                               x.ItemId,
                                                                                               x.Amount,
                                                                                               x.Invoice.EventId,
                                                                                               x.Invoice.VisitorId,
                                                                                               x.LineItemType)).ToList(),
                                         s.PassCategoryId,
                                         s.PassCategorySettingId,
                                         s.AppliedDiscountType,
                                         s.PassCategorySetToChargable,
                                         s.PaymentStatus));
    }
}
