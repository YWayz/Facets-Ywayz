namespace Facets.Core.TeamMembers.DTOs;

public sealed record QRVerifiedTeamMemberDto(string FirstName, string LastName, string IdentificationNumer, string PassCategory, string? profileImageURL);
