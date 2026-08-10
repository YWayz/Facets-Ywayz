using Ardalis.Specification;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorWithDocumentSpec : Specification<Visitor>
{
    public VisitorWithDocumentSpec(Guid visitorId, VisitorDocumentFilter filter)
    {
        Query.Where(w => w.Id == visitorId);

        Query.Include(i => i.Documents);

        if (filter.AttachmentTypes is not null) Query.Include(i => i.Documents.Where(w => filter.AttachmentTypes.Contains(w.AttachmentType)));
    }
}
