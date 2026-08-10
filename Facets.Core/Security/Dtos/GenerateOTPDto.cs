using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Security.Dtos;

public sealed record GenerateOTPDto(OTPType Type, string IdentityNumber, bool SendOTPByEmail);
