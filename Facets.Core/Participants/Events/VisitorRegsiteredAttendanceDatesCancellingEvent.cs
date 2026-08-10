using Facets.Core.Participants.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Participants.Events;

internal sealed class VisitorRegsiteredAttendanceDatesCancellingEvent : DomainEventBase
{
    public VisitorRegistration VisitorRegistration { get; }
    public List<VisitorAttendanceSchedule> AttendanceShedulesToCancel { get; }

    public VisitorRegsiteredAttendanceDatesCancellingEvent(VisitorRegistration visitorRegistration, List<VisitorAttendanceSchedule> attendanceShedulesToCancel) : base(isPrePersistantDomainEvent: true)
    {
        VisitorRegistration = visitorRegistration;
        AttendanceShedulesToCancel = attendanceShedulesToCancel;
    }
}
