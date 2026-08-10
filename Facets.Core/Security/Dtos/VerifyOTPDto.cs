using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Dtos;

public sealed record VerifyOTPDto(OTPType Type, string IdentityNumber, string Code);
