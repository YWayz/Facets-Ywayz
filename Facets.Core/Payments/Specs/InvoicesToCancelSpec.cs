using Ardalis.Specification;
using Facets.Core.Payments.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Specs;
internal sealed class InvoicesToCancelSpec : Specification<Invoice>
{
    public InvoicesToCancelSpec(Guid eventId, Guid visitorId, Guid visitorRegistrationId)
    {
        Query.Where(w => w.EventId == eventId &&
                         w.VisitorId == visitorId &&
                         w.VisitorRegistrationId == visitorRegistrationId &&
                         w.InvoiceCancelled == false &&
                         w.PaymentStatus == PaymentStatus.Unpaid);

        Query.Include(x => x.InvoiceLineItems);
    }
}