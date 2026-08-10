using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Microsoft.AspNetCore.Http;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.TeamMembers.Entities;

public sealed class TeamMemberDocument : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string UniqueName { get; private set; } = null!;
    public string DisplayName { get; private set; } = null!;

    public string AttachmentURL { get; private set; } = null!;
    public AttachmentType AttachmentType { get; private set; }

    public Guid TeamMemberId { get; private set; }
    public TeamMember TeamMember { get; private set; } = null!;

    private TeamMemberDocument() { }

    public TeamMemberDocument(string attachmentURL, AttachmentType attachmentType, string uniqueName, string displayName)
    {
        AttachmentURL = attachmentURL;
        AttachmentType = attachmentType;
        UniqueName = uniqueName;
        DisplayName = displayName;
    }

    public void Delete() => IsDeleted = true;
}
