namespace Facets.Core.Participants.DTOs;

internal sealed class UpdateVisitorToPavilionSessionInternalDto
{
    public required IEnumerable<Guid> PavilionSessionIDs { get; init; } = Enumerable.Empty<Guid>();
    public required Guid VisitorRegistrationId { get; init; }
}
