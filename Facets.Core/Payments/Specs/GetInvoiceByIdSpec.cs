using Ardalis.Specification;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;

namespace Facets.Core.Payments.Specs;

internal sealed class GetInvoiceByIdSpec : Specification<Invoice, InvoiceDto>
{
    public GetInvoiceByIdSpec(Guid invoiceId)
    {
        Query.Where(w => w.Id == invoiceId);

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
