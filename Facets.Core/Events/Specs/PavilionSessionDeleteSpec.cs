using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionSessionDeleteSpec : Specification<PavilionSession>
{
    public PavilionSessionDeleteSpec(Guid eventId, Guid pavilionId, Guid pavilionSessionId)
    {
        Query.Where(e => e.Id == pavilionSessionId && e.Pavilion.EventId == eventId && e.Id == pavilionSessionId);
    }
}
