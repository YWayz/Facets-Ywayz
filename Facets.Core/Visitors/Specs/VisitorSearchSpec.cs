using Ardalis.Specification;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorSearchSpec : Specification<Visitor, VisitorSearchDto>
{
    public VisitorSearchSpec(string? searchValue)
    {
        searchValue = searchValue?.ToLower();
        Query.Where(w => (string.IsNullOrWhiteSpace(w.NICNumber) == false && w.NICNumber.ToLower() == searchValue) ||
                         (string.IsNullOrWhiteSpace(w.PassportNumber) == false && w.PassportNumber.ToLower() == searchValue));

        Query.Select(s => new VisitorSearchDto(s.IsAssocifyMember,
                                               true,
                                               s.FirstName,
                                               s.LastName,
                                               s.NICNumber,
                                               s.PassportNumber,
                                               s.MobileNumber,
                                               s.Email,
                                               s.Id,
                                               s.VisitorStatus));
    }
}
