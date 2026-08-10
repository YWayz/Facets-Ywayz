using Ardalis.Specification;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Specs;

internal sealed class PassCategoryForActiveEventByIdSpec : Specification<PassCategory, PassCategoryDto>
{
    public PassCategoryForActiveEventByIdSpec(Guid eventId, Guid passCategoryId)
    {
        Query.Where(w => w.Id == passCategoryId && w.EventId == eventId && w.Event.Status == EventStatus.Active && w.IsDeleted == false);

        Query.Select(e => new PassCategoryDto
        (
         e.Id,
         e.EventId,
         e.Name,
         e.Description,
         e.VisitorPassCategoryType,
         e.Color,
         e.IsDefault,
         e.PassType!.Name,
         e.PassCategorySettings.Select(s => new PassCategorySettingsDto(s.Id,
                                                                        s.PassCategoryId,
                                                                        s.IsChargeable,
                                                                        s.Rate,
                                                                        s.DiscountedRate,
                                                                        s.ApplyEarlyRegistrationDiscountedRate,
                                                                        s.ApplyOnlineRegistrationDiscountedRate,
                                                                        s.ApplyEntireEventDiscountedRate,
                                                                        s.EarlyRegistrationDiscountedRateValidUntil,
                                                                        s.RateType))
                                                                        .ToList(),
         e.PassCategoryPavilionSettings.Select(s => new PassCategoryPavilionSettingsDto(s.Id,
                                                                                        s.PavilionRate,
                                                                                        s.PassCategoryId,
                                                                                        s.PavilionId,
                                                                                        s.PassCategory.Name,
                                                                                        s.PassCategory.PassType!.Name))
                                                                                        .ToList()));
    }
}
