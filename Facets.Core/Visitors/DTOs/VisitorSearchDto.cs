using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record VisitorSearchDto(bool IsAssocifyMember,
                                      bool IsRegisteredToFacets,
                                      string? FirstName,
                                      string? LastName,
                                      string? NICNumber,
                                      string? PassportNumber,
                                      string? MobileNumber,
                                      string? Email,
                                      Guid? VisitorId,
                                      VisitorStatus? VisitorStatus);
