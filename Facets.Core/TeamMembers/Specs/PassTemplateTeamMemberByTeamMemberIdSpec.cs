using Ardalis.Specification;
using Facets.Core.Participants.DTOs;
using Facets.Core.Participants.Entities;
using Facets.Core.TeamMembers.DTOs;
using Facets.Core.TeamMembers.Entities;
using Facets.SharedKernal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Facets.SharedKernal.Extensions;

namespace Facets.Core.TeamMembers.Specs;

internal class PassTemplateTeamMemberByTeamMemberIdSpec : Specification<TeamMemberEvent, PassTemplateTeamMemberDto>
{
    public PassTemplateTeamMemberByTeamMemberIdSpec(Guid teamMemberId, Guid eventId)
    {
        Query.Where(t => t.EventId == eventId && t.TeamMember.Id == teamMemberId);

        string currentDateTime = DateTimeOffset.UtcNow.GetLocalTime(AppConstants.SriLankaTimeZone).ToString("dd-MMM-yyyy, HH:mm");

        Query.Select(s => new PassTemplateTeamMemberDto
        (
            s.TeamMember.Id,
            s.TeamMember.FirstName,
            s.TeamMember.LastName,
            s.TeamMember.IdentityType,
            s.TeamMember.NICNumber,
            s.TeamMember.PassportNumber,
            s.TeamMember.MobileNumber,
            s.PassCategoryId,
            s.PassCategory.Name,
            currentDateTime,
            s.TeamMember.ImageURL!,
            s.Event.Name,
            s.PassCategory.Color,
            s.Event.EventDates.First(f => f.EventId == eventId).Date,
            s.TeamMember.CompanyName
        ));
    }
}