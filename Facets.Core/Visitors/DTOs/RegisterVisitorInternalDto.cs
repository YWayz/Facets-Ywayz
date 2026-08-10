using Facets.Core.Common.ValueObjects;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record RegisterVisitorInternalDto(VisitorIdentityType VisitorIdentityType,
                                                string? NICNumber,
                                                string? PassportNumber,
                                                Guid CountryId,
                                                string FirstName,
                                                string LastName,
                                                string MobileNumber,
                                                string? CompanyName,
                                                string? Email,
                                                AddressValueObject? Address,
                                                bool IsAssocifyMember,
                                                bool RegisterOnline);
