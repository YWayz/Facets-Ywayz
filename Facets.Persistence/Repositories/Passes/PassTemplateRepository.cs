using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Passes;

internal sealed class PassTemplateRepository : BaseRepository, IPassTemplateRepository
{
    private readonly DbSet<PassTemplate> _table;

    public PassTemplateRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<PassTemplate>();
    }

    public PassTemplate AddPassTemplate(PassTemplate entity)
    {
        _table.Add(entity);

        return entity;
    }

    public async Task<TResult?> GetProjectedPassTemplateSpec<TResult>(ISpecification<PassTemplate, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<PassTemplate?> GetPassTemplateSpec(ISpecification<PassTemplate> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var @event = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return @event;
    }

    public async Task<bool> IsPassTemplateAvailable(Guid eventId, AppEnums.PassType passType, CancellationToken cancellationToken)
    {
        var available = await _table.AnyAsync(w => w.IsDeleted == false &&
                                                   w.EventId == eventId &&
                                                   w.PassType == passType,
                                              cancellationToken);
        return available;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<PassTemplate, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }
}
