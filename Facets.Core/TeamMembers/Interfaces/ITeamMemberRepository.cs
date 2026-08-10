using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberRepository : IBaseRepository
{
    TeamMember AddTeamMember(TeamMember teamMember);

    Task<TeamMember?> GetTeamMemberBySpec(ISpecification<TeamMember> specification, CancellationToken token, bool asTracking = false);

    Task<TResult?> GetProjectedTeamMemberBySpec<TResult>(ISpecification<TeamMember, TResult> specification, CancellationToken token);

    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMember, TResult> specification, CancellationToken token);

}
