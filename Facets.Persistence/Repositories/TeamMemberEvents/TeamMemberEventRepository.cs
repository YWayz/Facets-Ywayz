using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Persistence.Repositories.TeamMemberEvents;

public sealed class TeamMemberEventRepository : BaseRepository, ITeamMemberEventRepository
{
    private readonly DbSet<TeamMemberEvent> _table;
    public TeamMemberEventRepository(AppDbContext dbContext) : base(dbContext)
    {
        _table = _dbContext.Set<TeamMemberEvent>();
    }

    public async Task<TeamMemberEvent?> GetTeamMemberEventBySpec(ISpecification<TeamMemberEvent> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var teamMember = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return teamMember;
    }
    public async Task<TResult?> GetProjectedTeamMemberEventBySpec<TResult>(ISpecification<TeamMemberEvent, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMemberEvent, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<PassTemplateTeamMemberDto> GetPassTemplateTeamMemberBySpec<PassTemplateTeamMemberDto>(ISpecification<TeamMemberEvent, PassTemplateTeamMemberDto> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);
        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<QRVerifiedTeamMemberDto?> GetPassToVerify(Guid eventId, Guid teamMemberId, CancellationToken token)
    {
        var teamMemberEvent = await _table.Where(w => w.TeamMember.IsDeleted == false &&
                                                      w.ActiveStatus == TeamMemberStatus.Active &&
                                                      w.EventId == eventId &&
                                                      w.TeamMemberId == teamMemberId &&
                                                      w.Event.Status == EventStatus.Active)
                                          .Select(s => new QRVerifiedTeamMemberDto(s.TeamMember.FirstName,
                                                                                   s.TeamMember.LastName,
                                                                                   s.TeamMember.PassportNumber! ?? s.TeamMember.NICNumber!,
                                                                                   s.PassCategory.Name,
                                                                                   s.TeamMember.ImageURL))
                                          .FirstOrDefaultAsync(token);

        return teamMemberEvent;
    }
}