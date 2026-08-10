using Facets.Core.Participants.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Events.Entities;

public sealed class PavilionSession : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public DateTimeOffset StartTime { get; private set; }
    public DateTimeOffset EndTime { get; private set; }
    public int AllowedVisitorCount { get; private set; }

    public Guid EventDateId { get; private set; }
    public EventDate EventDate { get; private set; } = null!;

    public Guid PavilionId { get; private set; }
    public Pavilion Pavilion { get; private set; } = null!;

    private readonly List<VisitorPavilionSessionAttendanceSchedule> _visitorPavilionSessionAttendanceSchedules = new();
    public IReadOnlyCollection<VisitorPavilionSessionAttendanceSchedule> VisitorPavilionSessionAttendanceSchedules => _visitorPavilionSessionAttendanceSchedules.AsReadOnly();

    private PavilionSession() { }

    public PavilionSession(Guid eventDateId, DateTimeOffset startTime, DateTimeOffset endTime, int allowedVisitorCount)
    {
        EventDateId = eventDateId;
        StartTime = startTime;
        EndTime = endTime;
        AllowedVisitorCount = allowedVisitorCount;
    }

    internal void Update(DateTimeOffset startTime, DateTimeOffset endTime, int allowedVisitorCount)
    {
        StartTime = startTime;
        EndTime = endTime;
        AllowedVisitorCount = allowedVisitorCount;
    }

    internal void Delete()
    {
        IsDeleted = true;
    }

    internal ResponseResult AddVisitor(Guid visitorRegistrationId,
                                       IEnumerable<Guid> pavilionSessionIDs,
                                       IEnumerable<Guid> visitorRegisteredEventDateIDs)
    {
        var hasVisitorRegsiteredToEventDate = visitorRegisteredEventDateIDs.Any(a => a == this.EventDateId);

        if (hasVisitorRegsiteredToEventDate is false)
            return new(new OperationFailedException("Pavilion Session",
                                                    "A pavilion session has been selected for a date to which the visitor has not registered"));

        var pavilionSessionId = pavilionSessionIDs.First(f => f == this.Id);

        _visitorPavilionSessionAttendanceSchedules.Add(new(visitorRegistrationId, pavilionSessionId));

        return new();
    }
}
