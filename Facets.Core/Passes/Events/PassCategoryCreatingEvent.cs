using Facets.Core.Passes.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Passes.Events;

internal sealed class PassCategoryCreatingEvent : DomainEventBase
{
    public PassCategory PassCategory { get; }

    public PassCategoryCreatingEvent(bool isPrePersistantDomainEvent, PassCategory passCategory) : base(isPrePersistantDomainEvent)
    {
        PassCategory = passCategory;
    }
}
