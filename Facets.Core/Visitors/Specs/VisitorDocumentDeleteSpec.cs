using Ardalis.Specification;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorDocumentDeleteSpec : Specification<Visitor>
{
    public VisitorDocumentDeleteSpec(Guid visitorId, Guid documentId)
    {
        Query.Where(w => w.Id == visitorId)
             .Include(w => w.Documents.Where(w => w.Id == documentId));
    }
}
