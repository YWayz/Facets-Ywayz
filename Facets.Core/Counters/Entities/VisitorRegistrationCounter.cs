using Facets.Core.Events.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Counters.Entities;

public sealed class VisitorRegistrationCounter : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; } = null!;

    public string? Description { get; private set; }

    public bool IsLocked { get; private set; }

    public CounterType CounterType { get; private set; } = CounterType.RegistrationAndPayment;

    public Guid EventId { get; set; }
    public Event Event { get; private set; } = null!;

    private readonly List<UserAssignedRegistrationCounter> _userAssignedRegistrationCounters = new();
    public IReadOnlyCollection<UserAssignedRegistrationCounter> UserAssignedRegistrationCounters => _userAssignedRegistrationCounters.AsReadOnly();

    private VisitorRegistrationCounter() { }

    public VisitorRegistrationCounter(string name, string? description, Guid eventId, CounterType counterType)
    {
        Name = name;
        Description = description;
        IsLocked = false;
        EventId = eventId;
        CounterType = counterType;
    }
    internal ResponseResult UpdateRegistrationCounterInfo(string name, string? description, CounterType counterType)
    {
        Name = name;
        Description = description;
        CounterType = counterType;
        return new();
    }

    internal ResponseResult Delete()
    {
        if (IsLocked is true) return new(new OperationFailedException(nameof(IsLocked), "Registration counter is locked"));

        IsDeleted = true;
        return new();
    }

    internal void AssignRegistrationCounter(Guid userId)
    {
        IsLocked = true;
        _userAssignedRegistrationCounters.Add(new(userId));
    }

    public void UnAssignRegistrationCounter(Guid unAssignedByUserId)
    {
        IsLocked = false;
        _userAssignedRegistrationCounters.FirstOrDefault()?.UnAssign(unAssignedByUserId);
    }

    internal ResponseResult Unlock(Guid currentUserId)
    {
        if (IsLocked is false) return new(new OperationFailedException("Counter", "Counter is already unlocked"));

        IsLocked = false;

        var assignment = _userAssignedRegistrationCounters.OrderByDescending(x => x.CreatedOn).First();

        assignment.UnAssign(currentUserId);

        return new();
    }
}
