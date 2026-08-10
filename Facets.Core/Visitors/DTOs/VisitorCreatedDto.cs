using Facets.Core.Common.ValueObjects;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record VisitorCreatedDto(Guid Id,
                                       bool IsAssocifyMember,
                                       VisitorIdentityType VisitorIdentityType,
                                       string? NICNumber,
                                       string? PassportNumber,
                                       Guid CountryId,
                                       bool RegisteredOnline,
                                       bool OTPVerified,
                                       bool OTPVerificationRequired,
                                       string FirstName,
                                       string LastName,
                                       string MobileNumber,
                                       string? CompanyName,
                                       string? Email,
                                       AddressValueObject? Address,
                                       VisitorStatus VisitorStatus);
