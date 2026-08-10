using Facets.Core.Participants.Entities;
using Facets.Core.Payments.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Payments.Events;

internal sealed class VisitorInvoicingEventEvent : DomainEventBase
{
    public VisitorRegistration VisitorRegistration { get; }
    public Invoice Invoice { get; }

    public VisitorInvoicingEventEvent(Invoice invoice) : base(isPrePersistantDomainEvent: true)
    {
        VisitorRegistration = invoice.VisitorRegistration;
        Invoice = invoice;
    }
}
