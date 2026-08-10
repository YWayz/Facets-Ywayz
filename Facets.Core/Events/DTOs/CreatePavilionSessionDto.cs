namespace Facets.Core.Events.DTOs;

public sealed record CreatePavilionSessionDto(IEnumerable<CreateOrUpdatePavilionSessionItemDto> PavilionSessions);