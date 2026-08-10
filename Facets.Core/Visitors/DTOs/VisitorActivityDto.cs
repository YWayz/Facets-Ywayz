using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.DTOs;

public sealed record VisitorActivityDto(Guid Id, DateTimeOffset CreatedOn, string Description, Guid VisitorId, Guid EventId, VisitorActivityType ActivityType);
