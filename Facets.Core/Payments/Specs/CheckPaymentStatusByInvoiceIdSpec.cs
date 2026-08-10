using Ardalis.Specification;
using Facets.Core.Payments.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Specs;

internal sealed class CheckPaymentStatusByInvoiceIdSpec : Specification<Invoice, PaymentStatus>
{
    public CheckPaymentStatusByInvoiceIdSpec(Guid invoiceId)
    {
        Query.Where(w => w.Id == invoiceId);

        Query.Select(s => s.PaymentStatus);
    }
}
