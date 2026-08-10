namespace Facets.Core.Participants.DTOs;

public sealed record RegisteredVisitorDto(Guid Id,
                                          Guid VisitorId,
                                          Guid PassCategoryId,
                                          IReadOnlyList<Guid> EventDateIDs,
                                          bool registeredToEventOnsite);
