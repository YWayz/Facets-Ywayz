using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.DTOs;

public sealed record PassTemplateVisitorDto(Guid VisitorId,
                              string FirstName,
                              string LastName,
                              VisitorIdentityType VisitorIdentityType,
                              string? NICNumber,
                              string? PassportNumber,
                              string MobileNumber,
                              Guid PassCategoryId,
                              string PassCategoryName,
                              string PassGeneratedDateTime,
                              DateTimeOffset PassDate,
                              string ProfileImage,
                              string EventName,
                              decimal PassRate,
                              string CountryName,
                              string? CompanyName,
                              string PassCategoryColor);


