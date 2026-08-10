using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Passes.Interfaces;

public interface IPassCategoryService
{
    Task<ResponseResult<PassCategoryDto>> CreatePassCategory(Guid eventId, CreatePassCategoryDto model, CancellationToken cancellationToken);
    Task<ResponseResult<IReadOnlyList<PassCategoryRateDto>>> GetPassCategoryRates(Guid eventId, Paginator paginator, PassCategorySettingsFilter filter, CancellationToken token);
    Task<ResponseResult<PassCategoryRateDetailDto>> GetPassCategoryRateById(Guid eventId, Guid passCategoryId, Guid id, CancellationToken token);
    Task<ResponseResult> UpdatePassCategoryRate(Guid eventId, Guid passCategoryId, Guid id, UpdatePassCategoryRateDto model, CancellationToken token);
    Task<ResponseResult> UpdateIsChargeableStatus(Guid eventId, Guid passCategoryId, Guid id, UpdatePassCategoryRateIsCharegableStatusDto model, CancellationToken token);
    Task<ResponseResult<PassCategoryDto>> GetPassCategoryById(Guid eventId, Guid id, CancellationToken token);
    Task<ResponseResult> UpdatePassCategory(Guid eventId, Guid passCategoryId, UpdatePassCategoryDto model, CancellationToken token);
    Task<ResponseResult> Delete(Guid eventId, Guid id, CancellationToken token);
    Task<ResponseResult<IReadOnlyList<PassCategoryDto>>> GetPassCategories(Guid eventId, Paginator paginator, PassCategoryFilter filter, CancellationToken token);
    Task<ResponseResult<PassCategoryDto>> GetTeamMemberPassCategoryById(Guid eventId, Guid id, CancellationToken token);

    Task<ResponseResult<PassCategoryDto>> GetPassCategoryForActiveEventById(Guid eventId, Guid id, CancellationToken token);
    Task<ResponseResult> UpdateVisitorPassCategoryType(Guid eventId, Guid passCategoryId, UpdateVisitorPassCategoryTypeDto model, CancellationToken token);
    Task<ResponseResult> UpdatePassCategoryPavilionRate(Guid eventId, Guid pavilionId, UpdatePassCategoryPavilionRateDto model, CancellationToken token);
}
