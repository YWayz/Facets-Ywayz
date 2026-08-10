using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record PassTemplateTeamMemberDto(Guid TeamMemberId,
                             string FirstName,
                             string LastName,
                             VisitorIdentityType VisitorIdentityType,
                             string? NICNumber,
                             string? PassportNumber,
                             string MobileNumber,
                             Guid PassCategoryId,
                             string PassCategoryName,
                             string PassGeneratedDateTime,
                             string ProfileImage,
                             string EventName,
                             string PassCategoryColor,
                             DateTimeOffset PassDate,
                             string? CompanyName);


