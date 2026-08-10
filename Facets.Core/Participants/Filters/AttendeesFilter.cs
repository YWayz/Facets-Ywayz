namespace Facets.Core.Participants.Filters;

public sealed record AttendeesFilter(string? SearchTerm, Guid? AttendanceSheduleId, bool IsPrinted);
