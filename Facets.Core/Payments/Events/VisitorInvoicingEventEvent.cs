using Facets.Core.Participants.Entities;
using Facets.Core.Payments.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Payments.Events;

/// <summary>Raised before the save: marks schedules and writes activity in the same transaction.</summary>
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

/// <summary>
/// Raised after the save has committed. Only side effects that must not happen for a rolled-back payment
/// belong here, such as sending the confirmation SMS/email with the QR codes.
/// </summary>
internal sealed class VisitorInvoicePaidEvent : DomainEventBase
{
    public VisitorRegistration VisitorRegistration { get; }
    public Invoice Invoice { get; }

    public VisitorInvoicePaidEvent(Invoice invoice) : base(isPrePersistantDomainEvent: false)
    {
        VisitorRegistration = invoice.VisitorRegistration;
        Invoice = invoice;
    }
}
