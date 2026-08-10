namespace Facets.Core.Events.DTOs;

public sealed record PavilionSessionSummaryDto(Guid Id,
                                               Guid EventDateId,
                                               DateTimeOffset StartTime,
                                               DateTimeOffset EndTime,
                                               int AllowedVisitorCount,
                                               Guid PavilionId,
                                               DateTimeOffset EventDate,
                                               bool IsVisitorRegistered);