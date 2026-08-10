using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.Events.Entities;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Filters;
using Facets.Core.Participants.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.Participants;

internal sealed class VisitorPavilionSessionRepository : BaseRepository, IVisitorPavilionSessionRepository
{
    private readonly DbSet<PavilionSession> _table;

    public VisitorPavilionSessionRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<PavilionSession>();
    }

    public async Task<IReadOnlyList<KeyValuePair<Guid, int>>> GetVisitorCountByPavilionSessions(IEnumerable<Guid> distinctPavilionSessionIds,
                                                                                                CancellationToken token)
    {
        var visitorPavilionSessionCount = await _table.Where(w => distinctPavilionSessionIds.Contains(w.Id))
                                                      .Select(s => new
                                                      {
                                                          PavilionSessionId = s.Id,
                                                          Count = s.VisitorPavilionSessionAttendanceSchedules
                                                                   .Where(w => w.Cancelled == false)
                                                                   .Count()
                                                      })
                                                      .Select(s => new KeyValuePair<Guid, int>(s.PavilionSessionId, s.Count))
                                                      .ToListAsync(token);

        return visitorPavilionSessionCount.AsReadOnly();
    }

    public async Task<IReadOnlyList<PavilionSession>> GetListBySpec(Ardalis.Specification.ISpecification<PavilionSession> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        query = query.WithSpecification(specification);

        var list = await query.ToListAsync(cancellationToken: token);

        return list;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Ardalis.Specification.ISpecification<PavilionSession, TResult> specification, CancellationToken token)
    {

        var query = _table.WithSpecification(specification);

        var projectedResult = await query.ToListAsync(cancellationToken: token);

        return (projectedResult, projectedResult.Count);
    }

    public async Task<(IReadOnlyCollection<PavilionSessionVisitorDto>, int)> GetPavilionSessionVisitors(Paginator paginator,
                                                                                                        Guid eventId,
                                                                                                        PavilionSessionVisitorFilter filter,
                                                                                                        CancellationToken cancellationToken)
    {
        var query = _dbContext.Set<VisitorPavilionSessionAttendanceSchedule>()
                              .Where(s => s.PavilionSession.Pavilion.EventId == eventId && s.IsDeleted == false);

        query = ApplyFilters(filter, query);

        var groupedQuery = query.GroupBy(g => (g.VisitorRegistration.Visitor.NICNumber ??
                                               g.VisitorRegistration.Visitor.PassportNumber) + '-' + g.VisitorRegistration.Visitor.FirstName + ' ' +
                                               g.VisitorRegistration.Visitor.LastName);

        var totalRecordCount = await query.Select(s => s.VisitorRegistrationId).Distinct().CountAsync();

        groupedQuery = groupedQuery.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                   .Take(paginator.PageSize);


        var pavilionSessionVisitors = await groupedQuery
                                            .Select(s => new PavilionSessionVisitorDto(s.Key, s.Select(a => new VisitorPavilionSessionsDto
                                                                                                                (a.PavilionSession.Pavilion.Name,
                                                                                                                 a.PavilionSession.EventDate.Date,
                                                                                                                 a.PavilionSession.EventDateId,
                                                                                                                 a.PavilionSession.StartTime,
                                                                                                                 a.PavilionSession.EndTime,
                                                                                                                 a.Cancelled)).ToList())
                                                                                                                ).ToListAsync(cancellationToken);

        return (pavilionSessionVisitors, totalRecordCount);

        IQueryable<VisitorPavilionSessionAttendanceSchedule> ApplyFilters(PavilionSessionVisitorFilter filter,
                                                                          IQueryable<VisitorPavilionSessionAttendanceSchedule> query)
        {
            if (string.IsNullOrWhiteSpace(filter.SearchTerm) is false)
            {
                query = query.Where(w => EF.Functions.Like(w.VisitorRegistration.Visitor.FirstName, filter.SearchTerm + "%") ||
                                         EF.Functions.Like(w.VisitorRegistration.Visitor.LastName, filter.SearchTerm + "%") ||
                                         EF.Functions.Like(w.VisitorRegistration.Visitor.NICNumber!, filter.SearchTerm + "%") ||
                                         EF.Functions.Like(w.VisitorRegistration.Visitor.PassportNumber!, filter.SearchTerm + "%"));
            }

            if (string.IsNullOrWhiteSpace(filter.PavilionName) is false)
                query = query.Where(w => EF.Functions.Like(w.PavilionSession.Pavilion.Name, filter.PavilionName + "%"));

            if (filter.EventDate is not null)
                query = query.Where(w => w.PavilionSession.EventDate.Date.Date == filter.EventDate!.Value.Date);

            if (filter.PavilionStatus is not null)
                query = query.Where(w => w.Cancelled == filter.PavilionStatus);

            return query;
        }
    }
}
