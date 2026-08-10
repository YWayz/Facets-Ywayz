namespace Facets.Core.Events.DTOs;

public sealed record PavilionSessionDto(Guid Id,
                                        Guid EventDateId,
                                        DateTimeOffset StartTime,
                                        DateTimeOffset EndTime,
                                        int AllowedVisitorCount,
                                        Guid PavilionId);
