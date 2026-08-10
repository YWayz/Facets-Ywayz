using Facets.Core.Common;
using Facets.Core.Common.Interfaces;
using Facets.Core.Events.Entities;
using Facets.Core.Participants.Events;
using Facets.Core.Passes.Entities;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.EventHandlers;
using Facets.Core.Visitors.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses; 

namespace Facets.Core.Participants.Entities;

public sealed class VisitorRegistration : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public Event Event { get; private set; } = null!;
    public Guid EventId { get; private set; }

    public Visitor Visitor { get; private set; } = null!;
    public Guid VisitorId { get; private set; }

    public bool RegisteredToEventOnsite { get; private set; }

    public Guid? VisitorRegistrationCounterId { get; private set; }

    public PassCategory PassCategory { get; private set; } = null!;
    public Guid PassCategoryId { get; private set; }

    private readonly List<VisitorAttendanceSchedule> _visitorAttendanceSchedules = new();
    public IReadOnlyCollection<VisitorAttendanceSchedule> VisitorAttendanceSchedules => _visitorAttendanceSchedules.AsReadOnly();

    private readonly List<VisitorPavilionSessionAttendanceSchedule> _visitorPavilionSessionAttendanceSchedules = new();
    public IReadOnlyCollection<VisitorPavilionSessionAttendanceSchedule> VisitorPavilionSessionAttendanceSchedules =>
                                                                         _visitorPavilionSessionAttendanceSchedules.AsReadOnly();

    public bool RegistrationCancelled { get; private set; } = false;
    public DateTimeOffset? RegistrationCancelledOn { get; private set; }

    private VisitorRegistration() { }

    public VisitorRegistration(Guid eventId,
                               Guid visitorId,
                               bool registeredToEventOnsite,
                               Guid passCategoryId,
                               Guid? visitorRegistrationCounterId,
                               IEnumerable<Guid> eventDateIds)
    {
        EventId = eventId;
        VisitorId = visitorId;
        RegisteredToEventOnsite = registeredToEventOnsite;
        PassCategoryId = passCategoryId;

        VisitorRegistrationCounterId = RegisteredToEventOnsite is true ? visitorRegistrationCounterId : null;

        foreach (var eventDateId in eventDateIds)
        {
            _visitorAttendanceSchedules.Add(new(eventDateId, attendanceScheduledOnsite: registeredToEventOnsite, visitorRegistrationCounterId));
        }
    }

    internal ResponseResult Update(IEnumerable<Guid> eventDateIds, bool attendanceScheduledOnsite, Guid? visitorRegistrationCounterId)
    {
        if (VisitorAttendanceSchedules.Where(w => w.IsInvoiced is true && w.Cancelled is false).Any(s => eventDateIds.Contains(s.EventDateId)))
            return new(new OperationFailedException("Event Dates", "One or more event date(s) has been already invoiced"));

        _visitorAttendanceSchedules.RemoveAll(a => a.IsInvoiced is false && a.Cancelled is false);

        foreach (var eventDateId in eventDateIds)
        {
            _visitorAttendanceSchedules.Add(new(eventDateId, attendanceScheduledOnsite, visitorRegistrationCounterId));
        }

        return new();
    }

    internal ResponseResult CancelRegistration(IEnumerable<Guid> attendanceScheduleIds)
    {
        if (_visitorAttendanceSchedules.Where(a => attendanceScheduleIds.Contains(a.Id)).Any(s => s.VisitorAttended is true))
            return new(new OperationFailedException("Registration Cancellation", "One or more event(s) has been attended and cannot be cancelled"));

        var attendanceShedulesToCancel = _visitorAttendanceSchedules.Where(w => attendanceScheduleIds.Contains(w.Id) && w.Cancelled is false).ToList();

        if (attendanceShedulesToCancel.Any() is false) return new();

        foreach (var attendanceSchedule in attendanceShedulesToCancel)
        {
            this._visitorPavilionSessionAttendanceSchedules
                .Where(w => w.PavilionSession.EventDateId == attendanceSchedule.EventDateId)
                .ToList()
                .ForEach(f => f.Cancel());

            attendanceSchedule.Cancel();
        }

        bool allAttendanceSchedulesCancelled = _visitorAttendanceSchedules.All(a => a.Cancelled);

        if (allAttendanceSchedulesCancelled is true)
        {
            RegistrationCancelled = true;
            RegistrationCancelledOn = DateTimeOffset.UtcNow;
        } 

         
        RegisterDomainEvent(new VisitorRegsiteredAttendanceDatesCancellingEvent(this, attendanceShedulesToCancel)); 
        return new();
 
    }
 

    internal ResponseResult UpdatePassPrinted()
    {
        VisitorAttendanceSchedules.First().MarkAsPrinted();

        return new();
    }
}
