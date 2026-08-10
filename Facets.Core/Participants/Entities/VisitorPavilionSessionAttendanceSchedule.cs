using Facets.Core.Events.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Participants.Entities;

public sealed class VisitorPavilionSessionAttendanceSchedule : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public VisitorRegistration VisitorRegistration { get; private set; } = null!;
    public Guid VisitorRegistrationId { get; private set; }

    public Guid PavilionSessionId { get; private set; }
    public PavilionSession PavilionSession { get; private set; } = null!;

    public bool Cancelled { get; private set; }

    public bool IsInvoiced { get; private set; }
    public bool CanAttend { get; private set; }

    public DateTimeOffset? CancelledOn { get; private set; }

    public bool VisitorAttended { get; private set; } = false;
    public DateTimeOffset? VisitorAttendedAt { get; private set; }

    private VisitorPavilionSessionAttendanceSchedule() { }

    public VisitorPavilionSessionAttendanceSchedule(Guid visitorRegistrationId, Guid pavilionSessionId)
    {
        VisitorRegistrationId = visitorRegistrationId;
        PavilionSessionId = pavilionSessionId;
    }

    internal void MarkAsCanAttendPavilionSession()
    {
        IsInvoiced = true;
        CanAttend = true;
    }

    internal ResponseResult UpdateAttendance()
    {
        if (Cancelled is true)
            return new(new OperationFailedException("Registration Cancelled", "Registration has been cancelled for this date"));

        if (VisitorAttended is false)
        {
            VisitorAttended = true;
            VisitorAttendedAt = DateTimeOffset.UtcNow;
        }

        return new();
    }

    internal void Cancel()
    {
        Cancelled = true;
        CancelledOn = DateTimeOffset.UtcNow;
    }
}
