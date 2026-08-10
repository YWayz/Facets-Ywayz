using Microsoft.AspNetCore.Mvc;

namespace Facets.Core.Participants.DTOs;

public sealed class UpdateVisitorOnlineEventRegistrationDto
{
    public IEnumerable<Guid> EventDateIDs { get; init; } = Enumerable.Empty<Guid>();

    [FromRoute]
    public Guid id { get; init; }

    public IEnumerable<Guid> PavilionSessionIDs { get; init; } = Enumerable.Empty<Guid>();
}
