using Facets.Core.Security.Entities;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.Counters.Entities;

public sealed class UserAssignedRegistrationCounter : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public Guid AssignedUserId { get; private set; }

    public UserProfile AssginedUser { get; private set; } = null!;

    public Guid VisitorRegistrationCounterId { get; private set; }
    public VisitorRegistrationCounter VisitorRegistrationCounter { get; private set; } = null!;

    public DateTimeOffset? UnAssignedAt { get; private set; }
    public Guid? UnAssignedByUserId { get; private set; }

    private UserAssignedRegistrationCounter() { }


    public UserAssignedRegistrationCounter(Guid userId)
    {
        AssignedUserId = userId;
    }

    internal void UnAssign(Guid unAssignedByUserId)
    {
        UnAssignedAt = DateTimeOffset.UtcNow;
        UnAssignedByUserId = unAssignedByUserId;
    }
}
