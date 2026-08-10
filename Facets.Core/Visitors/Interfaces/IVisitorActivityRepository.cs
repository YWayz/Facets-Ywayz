using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Visitors.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.Visitors.Interfaces;

public interface IVisitorActivityRepository : IBaseRepository
{
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorActivity, TResult> specification, CancellationToken token);
}
