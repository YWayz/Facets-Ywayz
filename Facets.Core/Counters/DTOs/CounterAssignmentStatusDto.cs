namespace Facets.Core.Counters.DTOs;

public sealed record CounterAssignmentStatusDto(bool hasCounterAssigned, Guid? AssignmentId, Guid? CounterId, Guid? AssignedUserId);
