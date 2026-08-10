using Ardalis.Specification;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Passes.Specs;

public sealed class PassCategoryRateUpdateIsChargeableStatusSpec : Specification<PassCategory>
{
    public PassCategoryRateUpdateIsChargeableStatusSpec(Guid eventId, Guid passCategoryId, Guid id)
    {
        Query.Where(e => e.Id == passCategoryId && e.EventId == eventId).Include(i => i.PassCategorySettings.Where(w => w.Id == id));
    }
}
