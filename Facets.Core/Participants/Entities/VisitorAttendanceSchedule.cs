using Facets.Core.Events.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Entities;

public sealed class VisitorAttendanceSchedule : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public EventDate EventDate { get; private set; } = null!;
    public Guid EventDateId { get; private set; }

    public VisitorRegistration VisitorRegistration { get; private set; } = null!;
    public Guid VisitorRegistrationId { get; private set; }

    public bool CanAttend { get; private set; }
    public bool VisitorAttended { get; private set; } = false;
    public DateTimeOffset? VisitorAttendedAt { get; private set; }
    public bool Cancelled { get; private set; } = false;
    public DateTimeOffset? CancelledOn { get; private set; }

    public bool IsInvoiced { get; private set; }

    private readonly List<VisitorAttendanceLog> _visitorAttendanceLog = new();
    public IReadOnlyCollection<VisitorAttendanceLog> VisitorAttendanceLog => _visitorAttendanceLog.AsReadOnly();

    public bool AttendanceScheduledOnsite { get; private set; }

    public Guid? RegsitrationCounterId { get; private set; }
    public bool IsPrinted { get; private set; }

    private VisitorAttendanceSchedule() { }

    public VisitorAttendanceSchedule(Guid eventDateId, bool attendanceScheduledOnsite, Guid? regsitrationCounterId)
    {
        EventDateId = eventDateId;
        VisitorAttended = false;
        CanAttend = false;
        IsInvoiced = false;
        AttendanceScheduledOnsite = attendanceScheduledOnsite;
        RegsitrationCounterId = regsitrationCounterId;
    }

    internal void Cancel()
    {
        Cancelled = true;
        CancelledOn = DateTimeOffset.UtcNow;
    }

    internal void MarkAsCanAttendEvent()
    {
        IsInvoiced = true;
        CanAttend = true;
    }

    internal ResponseResult UpdateAttendance(Guid qrScannedByUserId)
    {
        if (Cancelled is true)
            return new(new OperationFailedException("Registration Cancelled", "Registration has been cancelled for this date"));

        if (VisitorAttended is false)
        {
            VisitorAttended = true;
            VisitorAttendedAt = DateTimeOffset.UtcNow;
        }

        _visitorAttendanceLog.Add(new VisitorAttendanceLog(qrScannedByUserId));

        return new();
    }

    internal void MarkAsPrinted()
    {
        IsPrinted = true;
    }
}
