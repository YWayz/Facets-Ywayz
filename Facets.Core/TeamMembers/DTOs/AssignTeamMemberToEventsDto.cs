namespace Facets.Core.TeamMembers.DTOs;

public sealed record AssignTeamMemberToEventsDto(Guid teamMemberId, Guid eventId, Guid passCategoryId);
