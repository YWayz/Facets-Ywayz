namespace Facets.Core.Participants.DTOs;

public sealed record VisitorAttendedScheduleDto(Guid AttendanceScheduleId,
                                                DateTimeOffset RegisteredEventDate,
                                                bool VisitorAttended,
                                                DateTimeOffset? PassGeneratedAt,
                                                Guid EventDateId,
                                                bool Cancelled,
                                                string RegisteredMethod,
                                                DateTimeOffset RegisteredDateTime);
