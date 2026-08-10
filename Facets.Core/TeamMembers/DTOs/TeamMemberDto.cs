using Facets.Core.Common.ValueObjects;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberDto(Guid Id, 
                                   Title Title, 
                                   VisitorIdentityType IdentityType, 
                                   string? NICNumber, 
                                   string? PassportNumber, 
                                   Guid CountryId, 
                                   string CountryName, 
                                   string FirstName, 
                                   string LastName, 
                                   string MobileNumber, 
                                   string? Email,
                                   AddressValueObject? Address, 
                                   Guid? PassCategoryId,
                                   string? PassCategoryName, 
                                   string? ImageURL,
                                   string? CompanyName,
                                   TeamMemberStatus TeamMemberStatus,
                                   bool IsBlackListed );