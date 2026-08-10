using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Participants.DTOs;

public sealed record RegisteredVisitorDetailDto(Guid Id,
                                                Guid VisitorId,
                                                Guid PassCategoryId,
                                                IReadOnlyList<VisitorAttendanceScheduleDto> VisitorAttendanceSchedules,
                                                bool registeredToEventOnsite,
                                                Guid EventId,
                                                Guid? VisitorRegistrationCounterId,
                                                RateType RateType,
                                                VisitorPassCategoryType VisitorPassCategoryType,
                                                IReadOnlyList<VisitorPavilionSessionAttendanceScheduleDto> VisitorPavilionSessionAttendanceSchedules);
