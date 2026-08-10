using Ardalis.Specification;
using Facets.Core.Common.Interfaces;
using Facets.Core.Participants.Entities;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal.Models;

namespace Facets.Core.TeamMembers.Interfaces;

public interface ITeamMemberEventRepository : IBaseRepository
{
    Task<TeamMemberEvent?> GetTeamMemberEventBySpec(ISpecification<TeamMemberEvent> specification, CancellationToken token, bool asTracking = false);
    Task<TResult?> GetProjectedTeamMemberEventBySpec<TResult>(ISpecification<TeamMemberEvent, TResult> specification, CancellationToken token);
    Task<(IReadOnlyList<TResult> list, int totalRecords)> GetProjectedListBySpec<TResult>(Paginator paginator, ISpecification<TeamMemberEvent, TResult> specification, CancellationToken token);
    Task<PassTemplateTeamMemberDto> GetPassTemplateTeamMemberBySpec<PassTemplateTeamMemberDto>(ISpecification<TeamMemberEvent, PassTemplateTeamMemberDto> specification, CancellationToken token);

    Task<QRVerifiedTeamMemberDto?> GetPassToVerify(Guid eventId, Guid teamMemberId, CancellationToken token);

}
