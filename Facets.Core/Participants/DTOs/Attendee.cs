using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.DTOs;

public sealed record Attendee(Guid VisitorId,
                              string FirstName,
                              string LastName,
                              VisitorIdentityType VisitorIdentityType,
                              string? NICNumber,
                              string? PassportNumber,
                              string MobileNumber,
                              Guid PassCategoryId,
                              string PassCategoryName,
                              Guid AttendanceScheduleId,
                              Guid VisitorRegistrationId, 
                              VisitorPassCategoryType VisitorPassCategoryType,
                              bool IsInvoiced,
                              OnSitePayingMode OnSitePayingMode);
