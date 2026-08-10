using Facets.Core.Events.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Events.Events;

internal sealed class PavilionCreatingEvent : DomainEventBase
{
    public Pavilion Pavilion { get; }

    public PavilionCreatingEvent(bool isPrePersistantDomainEvent, Pavilion pavilion) : base(isPrePersistantDomainEvent)
    {
        Pavilion = pavilion;
    }
}
