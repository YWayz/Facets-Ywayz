using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberActivityDto(Guid Id, DateTimeOffset CreatedOn, string Description, Guid TeamMemberId, Guid EventId, TeamMemberActivityType ActivityType);
