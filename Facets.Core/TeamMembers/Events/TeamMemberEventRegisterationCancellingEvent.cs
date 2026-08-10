using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Events;

internal sealed class TeamMemberEventRegisterationCancellingEvent : DomainEventBase
{
    public TeamMemberEvent TeamMemberEvent { get; }
    public TeamMemberEventRegisterationCancellingEvent(TeamMemberEvent teamMemberEvent) : base(isPrePersistantDomainEvent: true)
    {
        TeamMemberEvent = teamMemberEvent;
    }
}
