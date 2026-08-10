namespace Facets.Core.Passes.DTOs;

public sealed record VisitorPavilionPassVerificationDto(Guid VisitorId,
                                                        Guid PavilionId,
                                                        Guid PavilionSessionId);