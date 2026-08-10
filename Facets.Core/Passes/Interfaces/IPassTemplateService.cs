using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Passes.Interfaces;

public interface IPassTemplateService
{
    Task<ResponseResult<PassTemplateDto>> CreatePassTemplate(Guid eventId, CreatePassTemplateDto model, CancellationToken cancellationToken);
    Task<ResponseResult> UpdatePassTemplate(Guid eventId, Guid passTemplateId, UpdatePassTemplateDto model, CancellationToken token);

    Task<ResponseResult<IReadOnlyList<PassTemplateDto>>> GetPassTemplates(Paginator paginator, Guid eventId, PassTemplateFilter filter, CancellationToken token);
}
