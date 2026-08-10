using Ardalis.Specification;
using Facets.Core.Events.Entities;

namespace Facets.Core.Participants.Specs;

internal sealed class AddVisitorToPavilsionSessionSpec : Specification<PavilionSession>
{
    public AddVisitorToPavilsionSessionSpec(IEnumerable<Guid> pavilionSessionIds)
    {
        Query.Where(w => pavilionSessionIds.Contains(w.Id));
    }
}