using Ardalis.Specification;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Filters;
using Microsoft.Extensions.Logging;
using static Facets.Core.Security.Claims.ApplicationClaimValues;

namespace Facets.Core.Passes.Specs;

public sealed class PassTemplateByIdSpec : Specification<PassTemplate, PassTemplateDto>
{
    public PassTemplateByIdSpec(Guid eventId, PassTemplateFilter filter)
    {
        Query.Where(w => w.PassType == filter.PassType && w.EventId == eventId);

        Query.Select(e => new PassTemplateDto
        (
             e.Id,
             e.TemplateText,
             e.PreviewTemplateText,
             e.Height,
             e.Width,
             e.PassType,
             e.EventId,
             e.SizeType
         ));
    }
}
