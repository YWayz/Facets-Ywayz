using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Dtos;

internal sealed record OTPInternalDto(Guid Id, OTPType Type, string IdentityNumber, string mobileNumber, string Code);
