using Ardalis.Specification;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Specs;

internal sealed class TeamMemberByIdAndEventIdSpec : Specification<TeamMember, TeamMemberDto>
{
    public TeamMemberByIdAndEventIdSpec(Guid teamMemberId, Guid eventId)
    {
        Query.Where(w => w.Id == teamMemberId && w.TeamMemberEvents.Any(a => a.EventId == eventId));

        Query.Select(s => new TeamMemberDto(s.Id,
                                            s.Title,
                                            s.IdentityType,
                                            s.NICNumber,
                                            s.PassportNumber,
                                            s.CountryId,
                                            s.Country.Name,
                                            s.FirstName,
                                            s.LastName,
                                            s.MobileNumber,
                                            s.Email,
                                            s.Address,
                                            s.TeamMemberEvents.First(w => w.EventId == eventId).PassCategoryId,
                                            s.TeamMemberEvents.First(w => w.EventId == eventId).PassCategory.Name,
                                            s.ImageURL,
                                            s.CompanyName,
                                            s.TeamMemberEvents.First(w => w.EventId == eventId).ActiveStatus,
                                            s.blacklistStatus.Equals(MemberBlacklistStatus.BlackListed) ? true: false
                                            ));
    }
}
