using Ardalis.Specification;
using Facets.Core.Visitors.Entities;

namespace Facets.Core.Visitors.Specs;

internal sealed class PublicVisitorOTPVerificationSpec : Specification<Visitor>
{
    public PublicVisitorOTPVerificationSpec(string identificationNumber)
    {
        identificationNumber = identificationNumber.ToUpper();

        Query.Where(w => w.RegisteredOnline &&
                         (w.NICNumber!.ToUpper() == identificationNumber || w.PassportNumber!.ToUpper() == identificationNumber));
    }
}
