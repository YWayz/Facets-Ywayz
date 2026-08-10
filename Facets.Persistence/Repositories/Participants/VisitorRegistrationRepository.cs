using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Participants;

internal sealed class VisitorRegistrationRepository : BaseRepository, IVisitorRegistrationRepository
{
    private readonly DbSet<VisitorRegistration> _table;

    public VisitorRegistrationRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<VisitorRegistration>();
    }

    public VisitorRegistration Add(VisitorRegistration visitorRegistration)
    {
        _table.Add(visitorRegistration);

        return visitorRegistration;
    }

    public async Task<VisitorRegistration?> FindById(Guid visitorRegistrationId)
    {
        return await _table.FindAsync(visitorRegistrationId);
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<VisitorRegistration, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<TResult?> GetProjectedRegistrationBySpec<TResult>(ISpecification<VisitorRegistration, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<VisitorRegistration?> GetRegistrationBySpec(ISpecification<VisitorRegistration> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var @event = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return @event;
    }
}
