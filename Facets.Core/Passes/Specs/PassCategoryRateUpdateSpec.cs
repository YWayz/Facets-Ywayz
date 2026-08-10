using Ardalis.Specification;
using Facets.Core.Passes.Entities;
using Facets.SharedKernal;

namespace Facets.Core.Passes.Specs;

public sealed class PassCategoryRateUpdateSpec : Specification<PassCategory>
{
    public PassCategoryRateUpdateSpec(Guid eventId, Guid passCategoryId, Guid id)
    {
        Query.Where(e => e.Id == passCategoryId && e.EventId == eventId && e.PassType!.Name == AppConstants.PassType.Visitor).Include(i => i.PassCategorySettings.Where(w => w.Id == id));
    }
}
