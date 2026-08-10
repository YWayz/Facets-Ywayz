namespace Facets.Core.Events.DTOs;

internal sealed record EventDateDetailDto(Guid EventId, Guid EventDateId, DateTimeOffset EventDate);
