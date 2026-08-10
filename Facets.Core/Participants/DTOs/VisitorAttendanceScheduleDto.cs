namespace Facets.Core.Participants.DTOs;

public sealed record VisitorAttendanceScheduleDto(Guid VisitorAttendanceScheduleId,
                                                  Guid EventDateId,
                                                  bool IsInvoiced,
                                                  bool IsCancelled);