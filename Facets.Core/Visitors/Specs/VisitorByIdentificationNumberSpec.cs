using Ardalis.Specification;
using Facets.Core.Visitors.DTOs;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class VisitorByIdentificationNumberSpec : Specification<Visitor, VisitorDto>
{
    public VisitorByIdentificationNumberSpec(string identificationNumber)
    {
        identificationNumber = identificationNumber.ToUpper();
        
        Query.Where(w => w.NICNumber!.ToUpper() == identificationNumber || w.PassportNumber!.ToUpper() == identificationNumber);

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
