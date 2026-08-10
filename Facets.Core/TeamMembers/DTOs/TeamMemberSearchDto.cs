using Facets.Core.Common.ValueObjects;
using Facets.SharedKernal;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.DTOs;

public sealed record TeamMemberSearchDto(
                                      bool IsRegisteredToFacets,
                                      string? FirstName,
                                      string? LastName,
                                      string? NICNumber,
                                      string? PassportNumber,
                                      string? MobileNumber,
                                      string? Email,
                                      string? ImageURL,
                                      AppEnums.Title Title,
                                      AddressValueObject? Address,
                                      Guid? PassCategoryId,
                                      Guid? CountryId,
                                      VisitorIdentityType IdentityType,
                                      AppEnums.TeamMemberStatus Status,
                                      Guid? TeamMemberId);

