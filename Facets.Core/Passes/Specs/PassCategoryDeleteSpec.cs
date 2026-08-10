using Ardalis.Specification;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Passes.Specs;

internal sealed class PassCategoryDeleteSpec : Specification<PassCategory>
{
    public PassCategoryDeleteSpec(Guid eventId, Guid id)
    {
        Query.Where(e => e.Id == id && e.EventId == eventId);
    }
}
