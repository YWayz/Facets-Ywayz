using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Interfaces;
using Facets.Infrastructure.OnePay.DTOs;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.Infrastructure.OnePay.Specs;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Extensions;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Responses;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Facets.Infrastructure.OnePay.Services;

internal sealed class OnePayService : IOnePayService
{
    private readonly OnePaySettings _onepaySettings;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOnePayAPIService _onePayAPIService;
    private readonly IOnePayRepository _onePayRepository;

    public OnePayService(IOptions<OnePaySettings> onepaySettings,
                         IInvoiceRepository invoiceRepository,
                         IOnePayAPIService onePayAPIService,
                         IOnePayRepository onePayRepository)
    {
        _onepaySettings = onepaySettings.Value;
        _invoiceRepository = invoiceRepository;
        _onePayAPIService = onePayAPIService;
        _onePayRepository = onePayRepository;
    }

    public async Task<ResponseResult<Gateway>> PaymentRequest(PaymentRequestDTO model, CancellationToken cancellationToken)
    {
        var invoiceDto = await _invoiceRepository.GetProjectedInvoiceBySpec(new InvoiceForOnePaySpec(model.InvoiceId), cancellationToken);

        if (invoiceDto is null) return new(new NotFoundException(nameof(model.InvoiceId), "Invoice", model.InvoiceId));

        OnePayPaymentRequestDto newPaymentRequest = new()
        {
            Currency = AppConstants.OnePay.ApplicableCurrency,
            Amount = decimal.Parse(OnePayHelper.FormatAmount(invoiceDto.TotalAmount), CultureInfo.InvariantCulture),
            AppId = _onepaySettings.AppID.Trim(),
            Reference = invoiceDto.ReferenceNumber,
            CustomerFirstName = invoiceDto.FirstName.RemoveWhitespaces(),
            CustomerLastName = invoiceDto.LastName.RemoveWhitespaces(),
            CustomerPhoneNumber = invoiceDto.MobileNumber.RemoveWhitespaces(), // v3 wants E.164 (+94...); "00" substitution was v1-specific
            customerEmail = invoiceDto.Email.RemoveWhitespaces(),
            TransactionRedirectUrl = $"{_onepaySettings.TransactionRedirectUrl}?invoiceId={invoiceDto.InvoiceId}",
            AdditionalData = $"invoiceId:{invoiceDto.InvoiceId};referenceNumber:{invoiceDto.ReferenceNumber}",
        };

        var response = await _onePayAPIService.RequestPayment(newPaymentRequest);

        if (response.Success is false) throw new OperationFailedException(response.Errors.First().Key, response.Errors.First().Value.First());

        LogRequestedPaymentResponse();

        await _onePayRepository.SaveChangesAsync(cancellationToken);

        return new(response.Data!.Gateway);

        void LogRequestedPaymentResponse()
        {
            var data = response.Data!;

            OnePayRequestedPaymentResponseLog requestedPaymentResponse = new(data.IPGTransactionId,
                                                                             data.Amount!.GrossAmount,
                                                                             data.Amount.HandlingFee,
                                                                             data.Amount.NetAmount,
                                                                             data.Amount.Currency,
                                                                             invoiceDto.InvoiceId,
                                                                             invoiceDto.ReferenceNumber);

            _onePayRepository.AddOnePayRequestedPaymentResponseLog(requestedPaymentResponse);
        }
    }

}
