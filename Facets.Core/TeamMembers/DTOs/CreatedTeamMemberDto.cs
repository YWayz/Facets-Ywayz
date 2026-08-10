using Facets.Core.Common.ValueObjects;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record CreatedTeamMemberDto(Guid Id, Title Title, VisitorIdentityType IdentityType, string? NICNumber, string? PassportNumber, Guid CountryId, string FirstName, string LastName, string MobileNumber, string? Email, AddressValueObject? Address, string? CompanyName);
