using Ardalis.Specification;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;
using Facets.Core.Visitors.Filters;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorListSpec : Specification<Visitor, VisitorSummaryDto>
{
    public VisitorListSpec(VisitorFilter filter)
    {
        Query.Select(s => new VisitorSummaryDto(s.Id,
                                                s.IsAssocifyMember,
                                                s.VisitorIdentityType,
                                                s.CountryId));
    }
}
