using Ardalis.Specification;
using Facets.Core.Payments.Entities;
using Facets.Infrastructure.OnePay.DTOs;

namespace Facets.Infrastructure.OnePay.Specs;

internal sealed class InvoiceForOnePaySpec : Specification<Invoice, OnePayInvoiceDto>
{
    public InvoiceForOnePaySpec(Guid invoiceId)
    {
        Query.Where(w => w.Id == invoiceId);

        Query.Select(w => new OnePayInvoiceDto(w.Id, 
                                               w.TotalAmount, 
                                               w.ReferenceNumber,
                                               w.VisitorRegistration.Visitor.FirstName,
                                               w.VisitorRegistration.Visitor.LastName,
                                               w.VisitorRegistration.Visitor.MobileNumber,
                                               w.VisitorRegistration.Visitor.Email!));
    }
}