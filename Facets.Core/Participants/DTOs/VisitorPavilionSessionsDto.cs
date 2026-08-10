namespace Facets.Core.Participants.DTOs;

public sealed record VisitorPavilionSessionsDto(string PavilionName, DateTimeOffset SessionDate, Guid SessionDateId, DateTimeOffset SessionStartTime, DateTimeOffset SessionEndTime, bool Cancelled);
