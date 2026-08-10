namespace Facets.Core.Events.DTOs;

public sealed record CreateOrUpdatePavilionSessionItemDto(Guid EventDateId,
                                                          DateTimeOffset StartTime,
                                                          DateTimeOffset EndTime,
                                                          int AllowedVisitorCount);