using Ardalis.Specification;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Passes.DTOs;

internal sealed class PassCategoryPavilionRateUpdateSpec : Specification<PassCategory>
{
    public PassCategoryPavilionRateUpdateSpec(Guid eventId)
    {
        Query.Where(e => e.EventId == eventId)
             .Include(i => i.PassCategoryPavilionSettings);
    }
}
