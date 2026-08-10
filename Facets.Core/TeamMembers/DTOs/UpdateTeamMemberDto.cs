using Facets.Core.Common.ValueObjects;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record UpdateTeamMemberDto(Title Title,
                                         VisitorIdentityType IdentityType,
                                         string? NicNumber,
                                         string? PassportNumber,
                                         Guid CountryId,
                                         string FirstName,
                                         string LastName,
                                         Guid EventId,
                                         Guid PassCategoryId,
                                         string MobileNumber,
                                         string? Email,
                                         string? ImageURL,
                                         string? CompanyName,
                                         AddressValueObject? Address);
