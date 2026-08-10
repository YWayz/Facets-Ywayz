using Ardalis.Specification;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Entities;
using Facets.SharedKernal;

namespace Facets.Core.Passes.Specs;

public sealed class PassCategoryRateByIdSpec : Specification<PassCategory, PassCategoryRateDetailDto>
{
    public PassCategoryRateByIdSpec(Guid eventId, Guid passCategoryId, Guid id)
    {
        Query.Where(w => w.Id == passCategoryId && w.EventId == eventId && w.PassType!.Name == AppConstants.PassType.Visitor)
             .Include(i => i.PassCategorySettings.Where(w => w.Id == id));

        Query.Select(e => new PassCategoryRateDetailDto
        (
            e.PassCategorySettings.First().Id,
            e.Id,
            e.Name,
            e.PassCategorySettings.First().IsChargeable,
            e.PassCategorySettings.First().Rate,
            e.PassCategorySettings.First().DiscountedRate,
            e.PassCategorySettings.First().ApplyEarlyRegistrationDiscountedRate,
            e.PassCategorySettings.First().ApplyOnlineRegistrationDiscountedRate,
            e.PassCategorySettings.First().ApplyEntireEventDiscountedRate,
            e.PassCategorySettings.First().EarlyRegistrationDiscountedRateValidUntil,
            e.EventId,
            e.Event.VisitorRegistrationStartsOn,
            e.Event.VisitorRegistrationEndsOn,
            e.PassCategorySettings.First().RateType
        ));
    }
}
