using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Entities;

public sealed class VisitorBlackListHistory : EntityBase, ICreatedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public VisitorStatus Status { get; private set; }

    public string Reason { get; private set; }

    public Guid VisitorId { get; private set; }
    public Visitor Visitor { get; private set; } = null!;

    public DateTimeOffset? BlackListedUntil { get; private set; }


    private VisitorBlackListHistory() { }

    public VisitorBlackListHistory(VisitorStatus status, string reason, DateTimeOffset? blackListedUntil)
    {
        Status = status;
        Reason = reason;
        BlackListedUntil = blackListedUntil;
    }
}
