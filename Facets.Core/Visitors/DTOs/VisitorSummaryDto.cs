using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record VisitorSummaryDto(Guid Id,
                                       bool IsAssocifyMember,
                                       VisitorIdentityType VisitorIdentityType,
                                       Guid CountryId);
