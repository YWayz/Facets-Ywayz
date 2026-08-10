using Ardalis.Specification;
using Facets.Core.Participants.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class GetUninvoicedAttendanceByRegistrationSpec : Specification<VisitorRegistration>
{
    public GetUninvoicedAttendanceByRegistrationSpec(Guid visitorId, Guid registrationId, Guid eventId)
    {
        Query.Where(w => w.RegistrationCancelled == false &&
                         w.IsDeleted == false &&
                         w.VisitorId == visitorId &&
                         w.Id == registrationId &&
                         w.EventId == eventId)
             .Include(s => s.VisitorAttendanceSchedules.Where(x => x.IsInvoiced == false &&
                                                                   x.Cancelled == false))
             .Include(s => s.VisitorPavilionSessionAttendanceSchedules.Where(w => w.Cancelled == false &&
                                                                                  w.IsInvoiced == false))
             .AsSplitQuery();
    }
}
