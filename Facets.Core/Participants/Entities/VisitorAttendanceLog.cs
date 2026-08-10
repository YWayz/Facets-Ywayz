using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;

namespace Facets.Core.Participants.Entities;

public sealed class VisitorAttendanceLog : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public VisitorAttendanceSchedule VisitorAttendanceSchedule { get; private set; } = null!;
    public Guid VisitorAttendanceScheduleId { get; private set; }

    public Guid QRScannedByUserId { get; private set; }

    private VisitorAttendanceLog() { }

    public VisitorAttendanceLog(Guid qrScannedByUserId)
    {
        QRScannedByUserId = qrScannedByUserId;
    }
}
