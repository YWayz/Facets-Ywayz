using Facets.Core.Events.Entities;
using Facets.SharedKernal;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Passes.Entities;

public sealed class PassTemplate : EntityBase, ICreatedAudit, IUpdatedAudit, IDeletedAudit, INoAudit
{
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; private set; }

    public string TemplateText { get; private set; } = null!;
    public string PreviewTemplateText { get; private set; } = null!;
    public decimal Height { get; private set; }
    public decimal Width { get; private set; }
    public TemplateSizeType SizeType { get; private set; }
    public AppEnums.PassType PassType { get; private set; }

    public Guid EventId { get; private set; }
    public Event Event { get; private set; } = null!;

    private PassTemplate() { }

    public PassTemplate(Guid eventId,
                        string tempateText,
                        string previewTemplateText,
                        decimal height,
                        decimal width,
                        AppEnums.PassType passType,
                        TemplateSizeType sizeType)
    {
        EventId = eventId;
        TemplateText = tempateText;
        PreviewTemplateText = previewTemplateText;
        Height = height;
        Width = width;
        PassType = passType;
        SizeType = sizeType;
    }

    internal void UpdatePassTemplateInfo(string tempateText,
                                                   string previewTemplateText,
                                                   decimal height,
                                                   decimal width,
                                                   TemplateSizeType sizeType)
    {
        TemplateText = tempateText;
        PreviewTemplateText = previewTemplateText;
        Height = height;
        Width = width;
        SizeType = sizeType;
    }
}
