using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Participants.Entities;
using Facets.Core.Passes.Entities;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Payments.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.Passes;

internal sealed class PassCategoryRepository : BaseRepository, IPassCategoryRepository
{
    private readonly DbSet<PassCategory> _table;

    public PassCategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<PassCategory>();
    }

    public PassCategory AddPassCategory(PassCategory entity)
    {
        _table.Add(entity);

        return entity;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<PassCategory, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<bool> IsPassCategoryNameTaken(Guid eventId, string name, Guid? id = null, CancellationToken cancellationToken = default)
    {
        return await _table.AnyAsync(f => f.EventId == eventId && f.Name == name && (id == null || f.Id != id.Value), cancellationToken: cancellationToken);
    }
    public async Task<PassCategory?> GetPassCategorySpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var result = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return result;
    }

    public async Task<IReadOnlyList<PassCategory>> GetPassCategoriesSpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var result = await query.WithSpecification(specification).ToListAsync(cancellationToken: token);

        return result;
    }

    public async Task<TResult?> GetProjectedPassCategorySpec<TResult>(ISpecification<PassCategory, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<TResult?> GetProjectedPassCategoryBySpec<TResult>(ISpecification<PassCategory, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<PassCategory?> GetPassCategoryBySpec(ISpecification<PassCategory> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var @event = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return @event;
    }

    public async Task<ResponseResult> CanDeletePassCategory(Guid id, CancellationToken token)
    {
        bool hasTeamMember = await _dbContext.Set<TeamMemberEvent>()
                                             .AnyAsync(s => s.PassCategoryId == id && s.ActiveStatus == TeamMemberStatus.Active, token);

        if (hasTeamMember)
            return new(new OperationFailedException("Delete Pass Category", "Cannot delete pass category as there are active team member(s)"));

        return new();
    }

    public async Task<IReadOnlyList<PassCategory>> GetPassCategories(Guid eventId, CancellationToken cancellationToken)
    {
        return await _table.Where(w => w.EventId == eventId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<VisitorPavilionRateDto>> GetPavilionRatesForPassCategoryByAttendanceScheduleId(Guid PassCategoryId,
                                                                                                   IEnumerable<Guid> visitoPavilionAttendanceIds,
                                                                                                   CancellationToken cancellationToken = new())
    {

        var vpsQuery = _dbContext.Set<VisitorPavilionSessionAttendanceSchedule>()
                            .Where(w => visitoPavilionAttendanceIds.Contains(w.Id) &&
                                        w.Cancelled == false &&
                                        w.IsDeleted == false)
                            .Select(s => new { VisitorPavilionSessionId = s.Id, s.PavilionSession.PavilionId });

        var pvsQuery = _dbContext.Set<PassCategoryPavilionSettings>().Where(w => w.PassCategoryId == PassCategoryId);

        var visitorPavilionRates = await vpsQuery.Join(pvsQuery,
                                                  vps => vps.PavilionId,
                                                  pvs => pvs.PavilionId,
                                                  (attSched, pavSettings) => new VisitorPavilionRateDto
                                                  {
                                                      VisitorPavilionSessionId = attSched.VisitorPavilionSessionId,
                                                      PavilionRate = pavSettings.PavilionRate
                                                  }).ToListAsync();

        return visitorPavilionRates.AsReadOnly();
    }

    public async Task<bool> CanUpdatePassRateType(Guid passCategoryId, Guid eventId, CancellationToken token)
    {
        var cannotUpdate = await _dbContext.Set<VisitorAttendanceSchedule>()
                                                              .Where(v => 
                                                               v.VisitorRegistration.EventId == eventId &&
                                                               v.VisitorRegistration.IsDeleted == false &&
                                                               v.VisitorRegistration.PassCategoryId == passCategoryId &&
                                                               v.VisitorRegistration.VisitorAttendanceSchedules.Any(s=>s.IsInvoiced == true) &&
                                                               v.VisitorRegistration.RegistrationCancelled == false)
                                        .AnyAsync(token);

        return cannotUpdate;
    }
}
