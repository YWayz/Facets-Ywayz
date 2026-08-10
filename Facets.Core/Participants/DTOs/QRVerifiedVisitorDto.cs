namespace Facets.Core.Participants.DTOs;

public sealed record QRVerifiedVisitorDto(string FirstName, string LastName, string IdentificationNumer, string PassCategory,string? profileImageURL);
