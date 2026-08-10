namespace Facets.Core.Visitors.DTOs;

public sealed record MarkAsBlackListedDto(DateTimeOffset? BlackListUntil, string Reason);
