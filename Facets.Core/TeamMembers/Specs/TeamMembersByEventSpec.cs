using Ardalis.Specification;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.Core.TeamMembers.Filters;
using Microsoft.EntityFrameworkCore;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Specs;

internal class TeamMembersByEventSpec : Specification<TeamMemberEvent, TeamMemberEventDto>
{
    public TeamMembersByEventSpec(TeamMemberEventFilter filter)
    {
        Query.Where(s => s.EventId == filter.EventId);

        if (string.IsNullOrWhiteSpace(filter.SearchQuery) is false)
        {
            Query.Where(w =>
                        EF.Functions.Like(w.TeamMember.NICNumber!, "%" + filter.SearchQuery + "%") ||
                        EF.Functions.Like(w.TeamMember.PassportNumber!, "%" + filter.SearchQuery + "%") ||
                        EF.Functions.Like(w.TeamMember.FirstName, "%" + filter.SearchQuery + "%") ||
                        EF.Functions.Like(w.TeamMember.LastName, "%" + filter.SearchQuery + "%") ||
                        EF.Functions.Like(w.TeamMember.MobileNumber, "%" + filter.SearchQuery + "%"))
                            .OrderByDescending(s => s.CreatedOn);
        }

        if (filter.PassCategoryIds is not null)
        {
            Query.Where(w => filter.PassCategoryIds.Contains(w.PassCategoryId));
        }

        if (filter.TeamMemberStatus is not null)
        {
            Query.Where(w => filter.TeamMemberStatus == w.ActiveStatus);
        }

        Query.Select(s => new TeamMemberEventDto(
            s.TeamMemberId,
            s.EventId,
            s.PassCategory.Name,
            s.TeamMember.FirstName + " " + s.TeamMember.LastName,
            s.TeamMember.blacklistStatus == MemberBlacklistStatus.BlackListed ? TeamMemberStatus.BlackListed : s.ActiveStatus,
            s.TeamMember.IdentityType,
            s.TeamMember.NICNumber,
            s.TeamMember.PassportNumber,
            s.TeamMember.MobileNumber
        ));
    }
}
