using Ardalis.Specification;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Entities;
using Facets.SharedKernal;

namespace Facets.Core.Passes.Specs;

public sealed class PassCategoryRateListSpec : Specification<PassCategory, PassCategoryRateDto>
{
    public PassCategoryRateListSpec(Guid eventId)
    {
        Query.Where(w => w.EventId == eventId && w.PassType!.Name == AppConstants.PassType.Visitor);

        Query.Select(e => new PassCategoryRateDto
        (
            e.PassCategorySettings.First().Id,
            e.Id,
            e.Name,
            e.PassCategorySettings.First().IsChargeable,
            e.PassCategorySettings.First().Rate,
            e.PassCategorySettings.First().DiscountedRate,
            e.EventId,
            e.Event.VisitorRegistrationStartsOn
        ));
    }
}
