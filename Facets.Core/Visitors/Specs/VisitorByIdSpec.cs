using Ardalis.Specification;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorByIdSpec : Specification<Visitor, VisitorDto>
{
    public VisitorByIdSpec(Guid visitorId)
    {
        Query.Where(w => w.Id == visitorId);

        Query.Select(s => new VisitorDto(s.Id,
                                         s.IsAssocifyMember,
                                         s.VisitorIdentityType,
                                         s.NICNumber,
                                         s.PassportNumber,
                                         s.CountryId,
                                         s.RegisteredOnline,
                                         s.OTPVerified,
                                         s.OTPVerificationRequired,
                                         s.FirstName,
                                         s.LastName,
                                         s.MobileNumber,
                                         s.CompanyName,
                                         s.Email,
                                         s.Address,
                                         s.VisitorStatus,
                                         s.Country.Name,
                                         s.VisitorReference));
    }
}
