using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Events.Entities;
using Facets.Core.Events.Interfaces;
using Facets.SharedKernal;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Events;

public sealed class PavilionRepository : BaseRepository, IPavilionRepository
{
    private readonly DbSet<Pavilion> _pavilionTable;
    private readonly DbSet<PavilionSession> _pavilionSessionTable;
    public PavilionRepository(AppDbContext dbContext) : base(dbContext)
    {
        _pavilionTable = _dbContext.Set<Pavilion>();
        _pavilionSessionTable = _dbContext.Set<PavilionSession>();
    }

    public Pavilion AddPavilion(Pavilion pavilion)
    {
        _pavilionTable.Add(pavilion);

        return pavilion;
    }

    public async Task<bool> IsPavilionNameTaken(Guid eventId, string name, CancellationToken cancellationToken, Guid? id = null)
    {
        var isNameTaken = await _pavilionTable.AnyAsync(a => a.Name == name && a.EventId == eventId && (id == null || a.Id != id.Value), cancellationToken);

        return isNameTaken;
    }

    public async Task<bool> CheckPavilionSessionsExist(Guid eventId, CancellationToken cancellationToken)
    {
        var isPavilionSessionsExist = await _pavilionTable.AnyAsync(a => a.EventId == eventId && a.PavilionSessions.Any());

        return isPavilionSessionsExist;
    }

    public async Task<TResult?> GetProjectedPavilionBySpec<TResult>(ISpecification<Pavilion, TResult> specification, CancellationToken token)
    {
        var query = _pavilionTable.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<Pavilion, TResult> specification, CancellationToken token)
    {
        var query = _pavilionTable.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<Pavilion?> GetPavilionBySpec(ISpecification<Pavilion> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _pavilionTable.AsTracking() : _pavilionTable;

        var pavilion = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return pavilion;
    }
    
    public async Task<PavilionSession?> GetPavilionSessionBySpec(ISpecification<PavilionSession> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _pavilionSessionTable.AsTracking() : _pavilionSessionTable;

        var pavilionSession = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return pavilionSession;
    }

    public async Task<bool> CheckOverlappingSession(Guid eventId, Guid pavilionId, Guid eventDateId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken, Guid? pavilionSessionId = null)
    {
        var startTimeLK = startTime.GetLocalTime(AppConstants.SriLankaTimeZone).TimeOfDay;
        var endTimeLK = endTime.GetLocalTime(AppConstants.SriLankaTimeZone).TimeOfDay;

        var query = _pavilionSessionTable.Where(w => w.Pavilion.EventId == eventId &&
                                                     w.EventDateId == eventDateId &&
                                                     w.PavilionId == pavilionId &&
                                                     (pavilionSessionId == null || w.Id != pavilionSessionId));

        var hasOverlap = await query.Where(x => 
                                           EF.Functions.AtTimeZone(x.StartTime, AppConstants.SriLankaTimeZone).TimeOfDay < endTimeLK &&
                                           EF.Functions.AtTimeZone(x.EndTime, AppConstants.SriLankaTimeZone).TimeOfDay > startTimeLK ||
                                           EF.Functions.AtTimeZone(x.StartTime, AppConstants.SriLankaTimeZone).TimeOfDay <= startTimeLK &&
                                           EF.Functions.AtTimeZone(x.EndTime, AppConstants.SriLankaTimeZone).TimeOfDay >= endTimeLK)
                                    .AnyAsync(cancellationToken);

        return hasOverlap;
    }

    public async Task<IReadOnlyList<Pavilion>> GetPavilions(Guid eventId, CancellationToken cancellationToken)
    {
        return await _pavilionTable.Where(w => w.EventId == eventId)
                                            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CanDeletePavilion(Guid eventId, Guid pavilionId, CancellationToken cancellationToken)
    {
        var pavilionHasSessionsOrVisitors = await _pavilionTable.Where(w => w.EventId == eventId && w.Id == pavilionId)
                                                                .AnyAsync(s => s.PavilionSessions
                                                                                .Any(m => m.VisitorPavilionSessionAttendanceSchedules
                                                                                           .Any()), cancellationToken);

        return !pavilionHasSessionsOrVisitors;
    }

    public async Task<bool> CanDeletePavilionSession(Guid eventId, Guid pavilionId, Guid pavilionSessionId, CancellationToken cancellationToken)
    {
        var pavilionHasSessionsOrVisitors = await _pavilionSessionTable.Where(w => w.Pavilion.EventId == eventId && w.Id == pavilionSessionId)
                                                                                    .AnyAsync(s => s.VisitorPavilionSessionAttendanceSchedules
                                                                                                    .Any(), cancellationToken);

        return !pavilionHasSessionsOrVisitors;
    }
}
