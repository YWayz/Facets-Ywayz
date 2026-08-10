using Ardalis.Specification;
using Facets.Core.Participants.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class UpdateVisitorRegistrationSpec : Specification<VisitorRegistration>
{
    public UpdateVisitorRegistrationSpec(Guid registrationId, Guid eventId)
    {
        Query.Where(w => w.Id == registrationId && w.EventId == eventId && w.RegistrationCancelled == false)
             .Include(s => s.VisitorAttendanceSchedules);
    }
}
