using Ardalis.Specification;
using Facets.Core.Payments.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Specs;

internal sealed class GetInvoicesToCreatePaymentSpec : Specification<Invoice>
{
    public GetInvoicesToCreatePaymentSpec(Guid registrationId, IEnumerable<Guid> invoiceIds, Guid eventId)
    {
        Query.Where(w => w.InvoiceCancelled == false &&
                         w.VisitorRegistrationId == registrationId &&
                         w.PaymentStatus == PaymentStatus.Unpaid &&
                         w.EventId == eventId &&
                         invoiceIds.Contains(w.Id));
    }
}
