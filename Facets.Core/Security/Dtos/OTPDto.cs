using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Dtos;

public sealed record OTPDto(Guid Id, OTPType Type, string IdentityNumber, string mobileNumber);
