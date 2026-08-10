using Ardalis.Specification.EntityFrameworkCore;
using Ardalis.Specification;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.EntityFrameworkCore;

namespace Facets.Persistence.Repositories.TeamMemberActivities;

public sealed class TeamMemberActivityRepository : BaseRepository, ITeamMemberActivityRepository
{
    private readonly DbSet<TeamMemberActivity> _table;
    public TeamMemberActivityRepository(AppDbContext dbContext): base(dbContext)
    {
        _table = _dbContext.Set<TeamMemberActivity>();
    }

    public void AddTeamMemberActivity(TeamMemberActivity teamMemberActivity)
    {
        _dbContext.Set<TeamMemberActivity>().Add(teamMemberActivity);
    }

    public async Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMemberActivity, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var totalRecords = await query.CountAsync(cancellationToken: token);

        var projectedResult = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
                                         .Take(paginator.PageSize)
                                         .ToListAsync(cancellationToken: token);

        return (projectedResult, totalRecords);
    }

    public async Task<TResult?> GetProjectedTeamMemberActivityBySpec<TResult>(ISpecification<TeamMemberActivity, TResult> specification, CancellationToken token)
    {
        var query = _table.WithSpecification(specification);

        var projectedResult = await query.FirstOrDefaultAsync(cancellationToken: token);

        return projectedResult;
    }
}
