using Ardalis.Specification;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;

namespace Facets.Core.TeamMembers.Specs;

internal class TeamMemberSearchSpec : Specification<TeamMember,  TeamMemberSearchDto>
{
    public TeamMemberSearchSpec(string? searchValue, Guid eventId)
    {
        searchValue = searchValue?.ToLower();
        Query.Where(w => (string.IsNullOrWhiteSpace(w.NICNumber) == false && w.NICNumber.ToLower() == searchValue) ||
                         (string.IsNullOrWhiteSpace(w.PassportNumber) == false && w.PassportNumber.ToLower() == searchValue))
            .Include(s=>s.TeamMemberEvents);

        Query.Select(s => new TeamMemberSearchDto(
                                               true,
                                               s.FirstName,
                                               s.LastName,
                                               s.NICNumber,
                                               s.PassportNumber,
                                               s.MobileNumber,
                                               s.Email,
                                               s.ImageURL,
                                               s.Title,
                                               s.Address,
                                               s.TeamMemberEvents.Where(e => e.EventId == eventId).Select(s => s.PassCategoryId).FirstOrDefault(),
                                               s.CountryId,
                                               s.IdentityType,
                                               s.TeamMemberEvents.Where(e => e.EventId == eventId).Select(s => s.ActiveStatus).FirstOrDefault(),
                                               s.Id));
    }
}
