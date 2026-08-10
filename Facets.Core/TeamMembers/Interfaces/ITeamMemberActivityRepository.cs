using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberActivityRepository : IBaseRepository
{
    void AddTeamMemberActivity(TeamMemberActivity teamMemberActivity);
    Task<TResult?> GetProjectedTeamMemberActivityBySpec<TResult>(ISpecification<TeamMemberActivity, TResult> specification, CancellationToken token);
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMemberActivity, TResult> specification, CancellationToken token);
}
