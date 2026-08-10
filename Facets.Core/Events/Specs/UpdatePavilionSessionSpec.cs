using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class UpdatePavilionSessionSpec : Specification<Pavilion>
{
    public UpdatePavilionSessionSpec(Guid eventId, Guid id, Guid pavilionSessionId)
    {
        Query.Where(w => w.EventId == eventId && w.Id == id)
             .Include(i => i.PavilionSessions.Where(w => w.Id == pavilionSessionId))
             .ThenInclude(ti => ti.VisitorPavilionSessionAttendanceSchedules.Where(w => w.PavilionSessionId == pavilionSessionId));
    }
}
