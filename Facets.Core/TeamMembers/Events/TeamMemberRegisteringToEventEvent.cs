using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Events;

internal sealed class TeamMemberRegisteringToEventEvent : DomainEventBase
{
    public TeamMemberEvent TeamMemberEvent { get; }

    public TeamMemberRegisteringToEventEvent(TeamMemberEvent teamMemberEvent) : base(isPrePersistantDomainEvent: true)
    {
        TeamMemberEvent = teamMemberEvent;
    }

}
