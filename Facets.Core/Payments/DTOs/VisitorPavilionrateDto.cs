namespace Facets.Core.Payments.DTOs;

public sealed class VisitorPavilionRateDto
{
    public Guid VisitorPavilionSessionId { get; init; }
    public decimal PavilionRate { get; init; }
}
