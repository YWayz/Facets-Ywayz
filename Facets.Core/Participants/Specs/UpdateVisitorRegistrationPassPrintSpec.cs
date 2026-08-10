using Ardalis.Specification;
using Facets.Core.Participants.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class UpdateVisitorRegistrationPassPrintSpec : Specification<VisitorRegistration>
{
    public UpdateVisitorRegistrationPassPrintSpec(Guid eventId, Guid visitorId, Guid attendanceScheduleId)
    {
        Query.Where(w => w.EventId == eventId && w.VisitorId == visitorId)
             .Include(i => i.VisitorAttendanceSchedules.Where(w => w.Id == attendanceScheduleId));
    }
}
