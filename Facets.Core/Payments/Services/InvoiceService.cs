using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Filters;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Payments.Interfaces.LineItemHadlers;
using Facets.Core.Payments.PaymentCalculationHandlers;
using Facets.Core.Payments.Specs;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Services;

internal sealed class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly ILoggedInUserService _loggedInUser;
    private readonly IEventDateLineItemHandler _eventDateLineItemHandler;
    private readonly IPavilionSessionLineItemHandler _pavilionSessionLineItemHandler;

    public InvoiceService(IInvoiceRepository invoiceRepository,
                          ILoggedInUserService loggedInUser,
                          IEventDateLineItemHandler eventDateLineItemHandler,
                          IPavilionSessionLineItemHandler pavilionSessionLineItemHandler)
    {
        _invoiceRepository = invoiceRepository;
        _loggedInUser = loggedInUser;
        _eventDateLineItemHandler = eventDateLineItemHandler;
        _pavilionSessionLineItemHandler = pavilionSessionLineItemHandler;
    }

    public async Task<ResponseResult<Invoice>> CreateInvoice(InternalCreateInvoiceDto internalModel, CancellationToken token)
    {
        var lineItemHandler = _eventDateLineItemHandler;
        lineItemHandler.SetNextHandler(_pavilionSessionLineItemHandler);

        var model = new PaymentCalculationHandlerModel(internalModel);

        var response = await lineItemHandler.Handle(model);

        if (response.Success is false) return new(response.Errors);

        if (model.InvoiceLineItems.Count is 0) return new(new OperationFailedException("Invoice", "No line items to create an invoice"));

        Invoice newInvoice = new(internalModel.VisitorRegistrationId,
                                internalModel.VisitorId,
                                internalModel.EventId,
                                internalModel.InvoicePaymentInfo.PassCategoryId,
                                internalModel.InvoicePaymentInfo.PassCategorySettingId,
                                model.ProcessedInvoiceData.DiscountType,
                                model.ProcessedInvoiceData.IsChargeable,
                                internalModel.RegistrationCounterId,
                                internalModel.InvoicedOnSite,
                                internalModel.InvoicePaymentInfo.RateType,
                                model.InvoiceLineItems);

        _invoiceRepository.Add(newInvoice);

        return new(newInvoice);
    }

    public async Task<ResponseResult<InvoiceDto>> GetInvoiceById(Guid id, CancellationToken token)
    {
        var invoice = await _invoiceRepository.GetProjectedInvoiceBySpec(new GetInvoiceByIdSpec(id), token);

        if (invoice is null) return new(new NotFoundException(nameof(id), "Invoice", id));

        return new(invoice);
    }

    public async Task<ResponseResult<IReadOnlyList<InvoiceDto>>> GetInvoicesByRegistration(Paginator paginator, RegistrationInvoiceFilter filter, Guid registrationId, CancellationToken token)
    {
        var (list, totalRecords) = await _invoiceRepository.GetProjectedListBySpec(paginator,
                                                                                   new GetInvoiceByRegistrationIdSpec(registrationId,
                                                                                                                      _loggedInUser.FacetsEventId,
                                                                                                                      filter),
                                                                                   token);

        return new(list, totalRecords);
    }

    public async Task<ResponseResult<InvoicePaymentDto>> InvoicePaymentByRegistration(Guid visitorId, InvoiceFilter filter, CancellationToken token)
    {
        var invoice = await _invoiceRepository.GetProjectedInvoiceBySpec(new VisitorInvoiceFilterSpec(_loggedInUser.FacetsEventId, visitorId, filter), token);

        if (invoice is null) return new(new NotFoundException(nameof(visitorId), "Invoice", visitorId));

        return new ResponseResult<InvoicePaymentDto>(invoice);
    }

    public async Task<ResponseResult<bool>> CheckPaymentStatus(Guid invoiceId, CancellationToken token)
    {
        var paymentStatus = await _invoiceRepository.GetProjectedInvoiceBySpec(new CheckPaymentStatusByInvoiceIdSpec(invoiceId), token);

        if (paymentStatus is PaymentStatus.None) return new(new NotFoundException(nameof(invoiceId), "Invoice", invoiceId));

        var isPaid = (paymentStatus is PaymentStatus.Paid or PaymentStatus.Free);

        return new(isPaid);
    }

    public async Task CancelUnpaidInvoices(Guid eventId, Guid visitorId, Guid visitorRegistrationId, CancellationToken token)
    {
        var invoices = await _invoiceRepository.GetInvoicesBySpec(new InvoicesToCancelSpec(eventId, visitorId, visitorRegistrationId),
                                                                  token,
                                                                  asTracking: true);

        foreach (var invoice in invoices)
        {
            invoice.CancelInvoice();
        }
    }
}
