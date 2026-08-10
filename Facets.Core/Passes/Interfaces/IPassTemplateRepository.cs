using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Passes.Entities;
using Facets.SharedKernal;
using Facets.SharedKernal.Models;

namespace Facets.Core.Passes.Interfaces;

public interface IPassTemplateRepository : IBaseRepository
{
    PassTemplate AddPassTemplate(PassTemplate entity);
    Task<TResult?> GetProjectedPassTemplateSpec<TResult>(ISpecification<PassTemplate, TResult> specification, CancellationToken token);
    Task<PassTemplate?> GetPassTemplateSpec(ISpecification<PassTemplate> specification, CancellationToken token, bool asTracking = false);
    Task<bool> IsPassTemplateAvailable(Guid eventId, AppEnums.PassType passType, CancellationToken cancellationToken);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<PassTemplate, TResult> specification, CancellationToken token);
}
