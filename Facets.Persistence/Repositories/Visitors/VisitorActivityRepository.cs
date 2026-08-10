using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Interfaces;
using Microsoft.EntityFrameworkCore;
using Facets.SharedKernal.Models;

namespace Facets.Persistence.Repositories.Visitors;

public sealed class VisitorActivityRepository : BaseRepository, IVisitorActivityRepository
{
    private readonly DbSet<VisitorActivity> _table;
    public VisitorActivityRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<VisitorActivity>();
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorActivity, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }
}
