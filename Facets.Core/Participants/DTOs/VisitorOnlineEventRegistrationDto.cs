namespace Facets.Core.Participants.DTOs;

public sealed class VisitorOnlineEventRegistrationDto
{
    public Guid VisitorId { get; init; }
    public Guid PassCategoryId { get; init; }
    public IEnumerable<Guid> EventDateIDs { get; init; } = Enumerable.Empty<Guid>();
    public IEnumerable<Guid> PavilionSessionIDs { get; init; } = Enumerable.Empty<Guid>();
}
