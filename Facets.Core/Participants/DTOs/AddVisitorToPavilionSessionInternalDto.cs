namespace Facets.Core.Participants.DTOs;

internal sealed class AddVisitorToPavilionSessionInternalDto
{
    public required IEnumerable<Guid> PavilionSessionIDs { get; init; } = Enumerable.Empty<Guid>();
    public required Guid VisitorRegistrationId { get; init; }

    public required Guid PassCategoryId { get; init; }
    public required IEnumerable<Guid> EventDateIDs { get; init; } = Enumerable.Empty<Guid>();
}
