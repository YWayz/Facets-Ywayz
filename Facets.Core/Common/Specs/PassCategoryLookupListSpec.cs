using Ardalis.Specification;
using Facets.Core.Common.Filters;
using Facets.Core.Passes.Entities;

namespace Facets.Core.Common.Specs;

internal sealed class PassCategoryLookupListSpec : Specification<PassCategory, KeyValuePair<Guid, string>>
{
    public PassCategoryLookupListSpec(Guid eventId, PassCategoryLookupFilter filter)
    {
        Query.Where(w => w.EventId == eventId && w.IsDeleted == false);

        if (filter.IsDefault.HasValue) Query.Where(s => s.IsDefault == filter.IsDefault.Value);

        if (filter.PassCategoryType is not null) Query.Where(s => filter.PassCategoryType.Contains(s.PassCategoryType));

        Query.Select(e => new KeyValuePair<Guid, string>
        (
             e.Id,
             e.Name
        ));
    }
}
