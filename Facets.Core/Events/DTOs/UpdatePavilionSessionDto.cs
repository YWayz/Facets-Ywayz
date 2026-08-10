namespace Facets.Core.Events.DTOs;

public sealed record UpdatePavilionSessionDto(IEnumerable<CreateOrUpdatePavilionSessionItemDto> PavilionSessions);