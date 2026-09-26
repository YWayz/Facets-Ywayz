using Facets.Core.Common.Validators;
using Facets.Core.Counters.Interfaces;
using Facets.Core.Events.Interfaces;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Payments.Validators;
using Facets.SharedKernal;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Interfaces;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Services;

internal sealed class PaymentService : IPaymentService
{
    private readonly ILoggedInUserService _loggedInUser;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IModelValidator _validator;
    private readonly IInvoiceStore _invoiceStore;
    private readonly IRegistrationCounterService _registrationCounterService;
    private readonly IEventSettingsService _eventSettingsService;

    public PaymentService(ILoggedInUserService loggedInUser,
                          IPaymentRepository paymentRepository,
                          IModelValidator validator,
                          IInvoiceStore invoiceStore,
                          IRegistrationCounterService registrationCounterService,
                          IEventSettingsService eventSettingsService)
    {
        _loggedInUser = loggedInUser;
        _paymentRepository = paymentRepository;
        _validator = validator;
        _invoiceStore = invoiceStore;
        _registrationCounterService = registrationCounterService;
        _eventSettingsService = eventSettingsService;
    }

    public async Task<ResponseResult> MakePaymentOnsite(CreateOnsitePaymentDto model, CancellationToken cancellationToken)
    {
        var userAssignedCounterResponse = await _registrationCounterService.GetUserAssignedRegistrationCounter(_loggedInUser.FacetsEventId,
                                                                                                               cancellationToken);

        if (userAssignedCounterResponse.Success is false) return new(userAssignedCounterResponse.Errors);

        var counterValidation = await ValidateCounter();

        if (counterValidation.Success is false) return new(counterValidation.Errors);

        var validationResult = await _validator.ValidateAsync<CreateOnsitePaymentDtoValidator, CreateOnsitePaymentDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        CreateInvoiceDto invoice = new(VisitorRegistrationId: model.RegistrationId,
                                       VisitorId: model.VisitorId,
                                       EventId: _loggedInUser.FacetsEventId,
                                       userAssignedCounterResponse.Data!.RegistrationCounterId,
                                       IsOnlinePayment: false,
                                       InvoicedOnSite: true);

        var invoiceResponse = await _invoiceStore.CreateInvoice(invoice, cancellationToken);

        if (invoiceResponse.Success is false) return new(invoiceResponse.Errors);

        if (invoiceResponse.Data!.TotalAmount != model.Amount)
            return new(new OperationFailedException("Payment", "Invoice total does not tally with paying amount"));

        Payment payment = new(model.PaymentMethod,
                              model.LastFourDigitsofCard,
                              model.ReferenceNumber,
                              invoiceResponse.Data!,
                              isOnlinePayment: false);

        _paymentRepository.Add(payment);

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        return new();

        async Task<ResponseResult> ValidateCounter()
        {
            var eventPaymentSettings = await _eventSettingsService.GetPaymentSetting(_loggedInUser.FacetsEventId, cancellationToken);

            if (eventPaymentSettings.Success is false) return new(eventPaymentSettings.Errors);

            if (eventPaymentSettings.Data!.OnSitePayingMode is OnSitePayingMode.PayAtRegistration)
            {
                if (userAssignedCounterResponse.Data!.CounterType is CounterType.RegistrationAndPayment) return new();

                return new(new OperationFailedException("Counter Type", "Payments must be done at a Registration and Payment Counter"));
            }

            if (eventPaymentSettings.Data!.OnSitePayingMode is OnSitePayingMode.PayAtPassGeneration)
            {
                if (userAssignedCounterResponse.Data!.CounterType is CounterType.RegistrationAndPayment or CounterType.PaymentOnly) return new();

                return new(new OperationFailedException("Counter Type",
                                                        "Payments must be done at a Registration and Payment Counter or Payment Only Counter"));
            }

            return new();
        }
    }

    public async Task<ResponseResult<PaymentDto>> MakePaymentOnline(CreateOnlinePaymentDto model, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync<CreateOnlinePaymentDtoValidator, CreateOnlinePaymentDto>(model, cancellationToken);

        if (validationResult.IsValid is false) return new(validationResult.Errors);

        CreateInvoiceDto invoice = new(VisitorRegistrationId: model.RegistrationId,
                                       VisitorId: model.VisitorId,
                                       EventId: _loggedInUser.FacetsEventId,
                                       RegistrationCounterId: null,
                                       IsOnlinePayment: true,
                                       InvoicedOnSite: false);

        var invoiceResponse = await _invoiceStore.CreateInvoice(invoice, cancellationToken);

        if (invoiceResponse.Success is false) return new(invoiceResponse.Errors);

        var invoiceData = invoiceResponse.Data!;

        var payingAmountValidationResponse = ValidatePayingAmount();

        if (payingAmountValidationResponse.Success is false) return new(payingAmountValidationResponse.Errors);

        AddPaymentInfoNoPaymentNeeded();

        await _paymentRepository.SaveChangesAsync(cancellationToken);

        PaymentDto paymentInfo = new(PaymentMethod.Card, IsOnlinePayment: true, invoiceData.Id, invoiceData.TotalAmount, invoiceData.PaymentStatus);

        return new(paymentInfo);

        void AddPaymentInfoNoPaymentNeeded()
        {
            if (invoiceData.PaymentStatus != PaymentStatus.Free) return;

            Payment payment = new(PaymentMethod.Card,
                                  model.LastFourDigitsOfCard,
                                  model.ReferenceNumber,
                                  invoiceResponse.Data!,
                                  isOnlinePayment: true);

            _paymentRepository.Add(payment);
        }

        ResponseResult ValidatePayingAmount()
        {
            if (invoiceData!.PaymentStatus != PaymentStatus.Free && invoiceData!.TotalAmount < AppConstants.OnePay.MinimumAmount)
                return new(new OperationFailedException("Payment", $"Minimum online paying amount must be equal to or greater than {AppConstants.OnePay.ApplicableCurrency} 100.00"));

            if (invoiceData!.TotalAmount != model.Amount)
                return new(new OperationFailedException("Payment", "Invoice total does not tally with paying amount"));

            return new();
        }
    }
}
