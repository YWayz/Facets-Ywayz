namespace Facets.Core.Participants.Filters;

public sealed record PavilionSessionVisitorFilter(string? SearchTerm, string? PavilionName, DateTimeOffset? EventDate, bool? PavilionStatus);
