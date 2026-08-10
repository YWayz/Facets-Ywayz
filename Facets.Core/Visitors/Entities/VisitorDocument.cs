using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Entities;

public sealed class VisitorDocument : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string AttachmentURL { get; private set; } = null!;
    public AttachmentType AttachmentType { get; private set; }

    public string UniqueName { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;

    public Visitor Visitor { get; set; } = null!;
    public Guid VisitorId { get; private set; }

    private VisitorDocument() { }

    public VisitorDocument(string attachmentURL, AttachmentType attachmentType, string uniqueName, string displayName)
    {
        AttachmentURL = attachmentURL;
        AttachmentType = attachmentType;
        UniqueName = uniqueName;
        DisplayName = displayName;
    }

    internal void Delete()
    {
        IsDeleted = true;
    }
}
