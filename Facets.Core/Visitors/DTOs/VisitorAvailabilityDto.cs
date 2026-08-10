using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record VisitorAvailabilityDto(bool Available,
                                            string IdentificationNumber,
                                            VisitorStatus VisitorStatus);
