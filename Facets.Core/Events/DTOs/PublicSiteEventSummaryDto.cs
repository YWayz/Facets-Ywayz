namespace Facets.Core.Events.DTOs;

public sealed record PublicSiteEventSummaryDto(Guid Id,
                                               string Name,
                                               string? Description,
                                               string? LogoURL,
                                               DateTimeOffset VisitorRegistrationStartDate,
                                               DateTimeOffset VisitorRegistrationEndDate,
                                               IReadOnlyList<DateTimeOffset> EventDates,
                                               bool IsUpComming);
