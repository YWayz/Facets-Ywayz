using Facets.Core.Visitors.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Visitors.Events;

internal sealed class VisitorCreatingEvent : DomainEventBase
{
    public Visitor Visitor { get; }

    public VisitorCreatingEvent(Visitor visitor) : base(isPrePersistantDomainEvent: true)
    {
        Visitor = visitor;
    }
}
