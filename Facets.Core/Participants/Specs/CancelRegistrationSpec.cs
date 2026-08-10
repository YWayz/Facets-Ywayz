using Ardalis.Specification;
using Facets.Core.Participants.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class CancelRegistrationSpec : Specification<VisitorRegistration>
{
    public CancelRegistrationSpec(Guid registrationId)
    {
        Query.Where(w => w.Id == registrationId && w.RegistrationCancelled == false)
             .Include(s => s.VisitorAttendanceSchedules)
                .ThenInclude(e => e.EventDate)
             .Include(s=>s.VisitorPavilionSessionAttendanceSchedules)
                .ThenInclude(ps=>ps.PavilionSession)
             .AsSplitQuery();
    }
}
