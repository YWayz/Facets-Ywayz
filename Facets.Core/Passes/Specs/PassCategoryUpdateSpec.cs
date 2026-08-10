using Ardalis.Specification;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Passes.Specs;

internal sealed class PassCategoryUpdateSpec : Specification<PassCategory>
{
    public PassCategoryUpdateSpec(Guid eventId, Guid passCategoryId)
    {
        Query.Where(e => e.Id == passCategoryId && e.EventId == eventId);
    }
}
