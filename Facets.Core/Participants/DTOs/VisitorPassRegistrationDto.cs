namespace Facets.Core.Participants.DTOs;

public sealed record VisitorPassRegistrationDto(IReadOnlyCollection<VisitorAttendedScheduleDto> AttendanceSchedules,
                                                string PassCategoryName,
                                                Guid VisitorRegistrationId,
                                                IReadOnlyCollection<VisitorPavilionSessionAttendanceScheduleDto> VisitorPavilionSessionAttendanceSchedules);