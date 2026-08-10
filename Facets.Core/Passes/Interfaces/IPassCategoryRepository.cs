using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Passes.Entities;
using Facets.Core.Payments.DTOs;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Passes.Interfaces;

public interface IPassCategoryRepository : IBaseRepository
{
    PassCategory AddPassCategory(PassCategory entity);
    Task<bool> IsPassCategoryNameTaken(Guid eventId, string name, Guid? id = null, CancellationToken cancellationToken = default);
    Task<PassCategory?> GetPassCategorySpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false);
    Task<TResult?> GetProjectedPassCategorySpec<TResult>(ISpecification<PassCategory, TResult> specification, CancellationToken token);
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<PassCategory, TResult> specification, CancellationToken token);
    Task<TResult?> GetProjectedPassCategoryBySpec<TResult>(ISpecification<PassCategory, TResult> specification, CancellationToken token);
    Task<PassCategory?> GetPassCategoryBySpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false);
    Task<ResponseResult> CanDeletePassCategory(Guid id, CancellationToken token);
    Task<IReadOnlyList<PassCategory>> GetPassCategoriesSpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false);
    Task<IReadOnlyList<PassCategory>> GetPassCategories(Guid eventId, CancellationToken cancellationToken);

    Task<IReadOnlyList<VisitorPavilionRateDto>> GetPavilionRatesForPassCategoryByAttendanceScheduleId(Guid PassCategoryId,
                                                                                                      IEnumerable<Guid> visitoPavilionAttendanceIds,
                                                                                                      CancellationToken cancellationToken = default);

    Task<bool> CanUpdatePassRateType(Guid passCategoryId, Guid eventId, CancellationToken token);
}