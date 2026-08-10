using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberEventDto(Guid TeamMemberId, Guid EventId, string PassCategory, string FullName, TeamMemberStatus Status, VisitorIdentityType IdentityType, string? NicNumber, string? PassportNumber, string MobileNumber);
