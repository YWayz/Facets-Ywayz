namespace Facets.Core.Participants.DTOs;

public sealed record CancelVisitorAttendanceDto(IEnumerable<Guid> AttendanceScheduleIds);
