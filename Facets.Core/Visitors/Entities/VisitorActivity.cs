using Facets.SharedKernal.Interfaces;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Entities;

public sealed class VisitorActivity : INoAudit, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public Guid Id { get; set; }
    public Guid VisitorId { get; private set; }
    public Visitor Visitor { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    public Guid FacetsEventId { get; private set; }
    public VisitorActivityType VisitorActivityType { get; private set; }

    private VisitorActivity() { }

    public VisitorActivity(Guid eventId, Guid visitorId, string description, VisitorActivityType visitorActivityType)
    {
        FacetsEventId = eventId;
        VisitorId = visitorId;
        Description = description;
        VisitorActivityType = visitorActivityType;
    }
}
