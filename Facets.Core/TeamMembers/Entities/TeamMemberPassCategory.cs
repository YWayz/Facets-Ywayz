using Facets.Core.Passes.Entities;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Entities;

public sealed class TeamMemberPassCategory : EntityBase, ICreatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public Guid TeamMemberId { get; private set; }
    public TeamMember TeamMember { get; private set; } = null!;

    public Guid PassCategoryId { get; private set; }
    public PassCategory PassCategory { get; private set; } = null!;

    private TeamMemberPassCategory() { }

    public TeamMemberPassCategory(Guid passCategoryId)
    {
        PassCategoryId = passCategoryId;
    }
}
