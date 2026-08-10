using Facets.Core.Events.Entities;
using Facets.Core.Passes.Entities;
using Facets.Core.TeamMembers.Events;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Entities;

public sealed class TeamMemberEvent : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }

    public Guid TeamMemberId { get; private set; }
    public TeamMember TeamMember { get; private set; } = null!;

    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;

    public Guid PassCategoryId { get; private set; }
    public PassCategory PassCategory { get; set; } = null!;

    public TeamMemberStatus ActiveStatus { get; private set; } = TeamMemberStatus.Active;

    private TeamMemberEvent() { }

    public TeamMemberEvent(Guid eventId, Guid passCategoryId)
    {
        EventId = eventId;
        PassCategoryId = passCategoryId;
        RegisterDomainEvent(new TeamMemberRegisteringToEventEvent(this));
    }

    public void AssignTeamMemberPassCategory(Guid passCategoryId)
    {
        PassCategoryId = passCategoryId;
        RegisterDomainEvent(new TeamMemberRegisteringToEventEvent(this));
    }

    public void CancelTeamMemberFromEvent()
    {
        ActiveStatus = TeamMemberStatus.Cancelled;
        RegisterDomainEvent(new TeamMemberEventRegisterationCancellingEvent(this));
    }

    public void AddTeamMemberToEvent()
    {
        ActiveStatus = TeamMemberStatus.Active;
        RegisterDomainEvent(new TeamMemberRegisteringToEventEvent(this));
    }
}
