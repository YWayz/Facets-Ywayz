using Facets.SharedKernal.Interfaces;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Entities;

public sealed class TeamMemberActivity : INoAudit, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public Guid Id { get; set; }
    public Guid TeamMemberId { get; private set; }
    public TeamMember TeamMember { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public Guid FacetsEventId { get; private set; }

    public TeamMemberActivityType ActivityType { get; set; } = TeamMemberActivityType.None;

    private TeamMemberActivity() { }

    public TeamMemberActivity(Guid eventId, Guid teamMemberId, string description, TeamMemberActivityType activityType)
    {
        FacetsEventId = eventId;
        TeamMemberId = teamMemberId;
        Description = description;
        ActivityType = activityType;
    }
}
