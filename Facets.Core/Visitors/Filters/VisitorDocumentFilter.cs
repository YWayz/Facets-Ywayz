using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Visitors.Filters;

public sealed record VisitorDocumentFilter
{
    public IEnumerable<AttachmentType>? AttachmentTypes { get; init; }
}
