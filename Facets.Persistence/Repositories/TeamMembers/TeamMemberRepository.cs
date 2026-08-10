using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.TeamMembers;

public sealed class TeamMemberRepository: BaseRepository, ITeamMemberRepository
{
    private readonly DbSet<TeamMember> _table;
    public TeamMemberRepository(AppDbContext dbContext): base(dbContext)
    {
        _table = _dbContext.Set<TeamMember>();
    }

    public TeamMember AddTeamMember(TeamMember teamMember)
    {
        _table.Add(teamMember);
        return teamMember;
    }

    public async Task<TeamMember?> GetTeamMemberBySpec(ISpecification<TeamMember> specification, CancellationToken token, bool asTracking = false)
    {
        var query = asTracking ? _table.AsTracking() : _table;

        var teamMember = await query.WithSpecification(specification).FirstOrDefaultAsync(cancellationToken: token);

        return teamMember;
    }
    public async Task<TResult?> GetProjectedTeamMemberBySpec<TResult>(ISpecification<TeamMember, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMember, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }    
}
