namespace Facets.Core.Participants.DTOs;

public sealed record VisitorPavilionSessionAttendanceScheduleDto(Guid VisitorPavilionAttendanceScheduleId,
                                                                 Guid EventDateId,
                                                                 Guid PavilionId,
                                                                 Guid PavilionSessionId,
                                                                 bool IsCancelled,
                                                                 bool IsInvoiced,
                                                                 string PavilionName,
                                                                 DateTimeOffset PavilionSessionDate,
                                                                 DateTimeOffset StartTime,
                                                                 DateTimeOffset EndTime,
                                                                 string RegisteredMethod,
                                                                 DateTimeOffset RegisteredDateTime,
                                                                 DateTimeOffset? PassGeneratedAt);