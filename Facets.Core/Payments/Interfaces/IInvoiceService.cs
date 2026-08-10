using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Filters;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Payments.Interfaces;

public interface IInvoiceService
{
    Task<ResponseResult<InvoiceDto>> GetInvoiceById(Guid id, CancellationToken token);

    Task<ResponseResult<IReadOnlyList<InvoiceDto>>> GetInvoicesByRegistration(Paginator paginator, RegistrationInvoiceFilter filter, Guid registrationId, CancellationToken token);

    internal Task<ResponseResult<Invoice>> CreateInvoice(InternalCreateInvoiceDto internalModel, CancellationToken token);

    Task<ResponseResult<InvoicePaymentDto>> InvoicePaymentByRegistration(Guid visitorRegistrationId, InvoiceFilter filter, CancellationToken token);
    Task<ResponseResult<bool>> CheckPaymentStatus(Guid invoiceId, CancellationToken token);
    Task CancelUnpaidInvoices(Guid eventId, Guid visitorId, Guid visitorRegistrationId, CancellationToken token);
}
