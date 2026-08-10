using Ardalis.Specification;
using Facets.Core.Events.DTOs;
using Facets.Core.Events.Entities;

namespace Facets.Core.Events.Specs;

internal sealed class PavilionByIdSpec : Specification<Pavilion, PavilionDto>
{
    public PavilionByIdSpec(Guid eventId, Guid id)
    {
        Query.Where(w => w.EventId == eventId && w.Id == id);

        Query.Select(s => new PavilionDto
        (
         s.Id,
         s.Name,
         s.PassCategoryPavilionSettings.Select(s => new Passes.DTOs.PassCategoryPavilionSettingsDto(s.Id,
                                                                                                    s.PavilionRate,
                                                                                                    s.PassCategoryId,
                                                                                                    s.PavilionId,
                                                                                                    s.PassCategory.Name,
                                                                                                    s.PassCategory.PassType!.Name
                                                                                                    )).ToList()
        ));
    }
}
