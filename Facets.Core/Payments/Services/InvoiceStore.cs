using Facets.Core.Events.Interfaces;
using Facets.Core.Participants.Entities;
using Facets.Core.Participants.Interfaces;
using Facets.Core.Participants.Specs;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Entities;
using Facets.Core.Payments.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Services;

internal sealed class InvoiceStore : IInvoiceStore
{
    private readonly IEventService _eventService;
    private readonly IPassCategoryService _passCategoryService;
    private readonly IInvoiceService _invoiceService;
    private readonly IVisitorRegistrationRepository _visitorRegistrationRepository;

    public InvoiceStore(IEventService eventService,
                        IPassCategoryService passCategoryService,
                        IInvoiceService invoiceService,
                        IVisitorRegistrationRepository visitorRegistrationRepository)
    {
        _eventService = eventService;
        _passCategoryService = passCategoryService;
        _invoiceService = invoiceService;
        _visitorRegistrationRepository = visitorRegistrationRepository;
    }

    public async Task<ResponseResult<Invoice>> CreateInvoice(CreateInvoiceDto model, CancellationToken cancellationToken)
    {
        var registration = await GetUnInvoicedItemsByRegistrationId();

        if (registration is null) return new(new OperationFailedException("Registration", "Registration not found"));

        // If an open invoice already covers exactly these items, hand it back instead of cancelling it.
        // A cancelled invoice may still have a live OnePay link; if the visitor completed that link the
        // payment could not be applied. Reusing the invoice keeps one link and one price per set of items.
        var openInvoices = await _invoiceService.GetOpenInvoices(model.EventId, model.VisitorId, model.VisitorRegistrationId, cancellationToken);

        var currentItemIds = registration.VisitorAttendanceSchedules.Select(s => s.Id)
                                         .Concat(registration.VisitorPavilionSessionAttendanceSchedules.Select(s => s.Id))
                                         .ToHashSet();

        var reusable = openInvoices.FirstOrDefault(i => i.InvoicedOnSite == model.InvoicedOnSite
                                                        && i.InvoiceLineItems.Where(l => l.IsDeleted is false).Select(l => l.ItemId).ToHashSet().SetEquals(currentItemIds));

        if (reusable is not null) return new(reusable);

        await _invoiceService.CancelUnpaidInvoices(model.EventId, model.VisitorId, model.VisitorRegistrationId, cancellationToken);

        var eventDatesResponse = await ValidateEventDates();

        if (eventDatesResponse.Success is false) return new(eventDatesResponse.Errors);

        var passCateggoryResponse = await _passCategoryService.GetPassCategoryForActiveEventById(model.EventId,
                                                                                                 registration.PassCategoryId,
                                                                                                 cancellationToken);

        if (passCateggoryResponse.Success is false) return new(passCateggoryResponse.Errors);

        var passCategorySetting = passCateggoryResponse.Data!.PassCategorySettings.FirstOrDefault();

        if (passCategorySetting is null) return new(new OperationFailedException(nameof(passCategorySetting), "Pass category setting is not available"));

        InternalCreateInvoiceDto internalModel = MapLineItemSetsAndInvoiceData();

        var invoice = await _invoiceService.CreateInvoice(internalModel, cancellationToken);

        return invoice;

        InternalCreateInvoiceDto MapLineItemSetsAndInvoiceData()
        {
            Dictionary<InvoiceLineItemType, IEnumerable<Guid>> invoiceLineItemSets = new();

            invoiceLineItemSets[InvoiceLineItemType.VisitorEventAttendance] = registration.VisitorAttendanceSchedules.Select(s => s.Id).ToList();

            invoiceLineItemSets[InvoiceLineItemType.VisitorPavilionSessionAttendance] = registration
                                                                                        .VisitorPavilionSessionAttendanceSchedules!
                                                                                        .Select(s => s.Id)
                                                                                        .ToList();

            InternalCreateInvoiceDto internalModel = new(VisitorRegistrationId: model.VisitorRegistrationId,
                                                         VisitorId: model.VisitorId,
                                                         EventId: model.EventId,
                                                         invoiceLineItemSets,
                                                         SelectedEventDateIds: registration.VisitorAttendanceSchedules.Select(s => s.EventDateId),
                                                         EventDateIds: eventDatesResponse.Data!,
                                                         RegistrationCounterId: model.RegistrationCounterId,
                                                         IsOnlinePayment: model.IsOnlinePayment,
                                                         InvoicedOnSite: model.InvoicedOnSite,
                                                         InvoicePaymentInfo: new(passCategorySetting!.Id,
                                                                                 passCategorySetting.PassCategoryId,
                                                                                 passCategorySetting.IsChargeable,
                                                                                 passCategorySetting.Rate,
                                                                                 passCategorySetting.DiscountedRate,
                                                                                 passCategorySetting.ApplyEarlyRegistrationDiscountedRate,
                                                                                 passCategorySetting.ApplyOnlineRegistrationDiscountedRate,
                                                                                 passCategorySetting.ApplyEntireEventDiscountedRate,
                                                                                 passCategorySetting.EarlyRegistrationDiscountedRateValidUntil,
                                                                                 passCategorySetting.RateType));
            return internalModel;
        }

        async Task<VisitorRegistration?> GetUnInvoicedItemsByRegistrationId()
        {
            return await _visitorRegistrationRepository
                         .GetRegistrationBySpec(new GetUninvoicedAttendanceByRegistrationSpec(visitorId: model.VisitorId,
                                                                                              registrationId: model.VisitorRegistrationId,
                                                                                              model.EventId),
                                                cancellationToken,
                                                asTracking: true);
        }

        async Task<ResponseResult<IEnumerable<Guid>>> ValidateEventDates()
        {
            var eventResponse = await _eventService.GetEventById(model.EventId, cancellationToken);

            if (eventResponse.Success is false) return new(eventResponse.Errors);

            var eventDateIds = eventResponse.Data!.EventDates.Select(s => s.Key).ToList();

            if (registration!.VisitorAttendanceSchedules.Select(s => s.EventDateId).All(a => eventDateIds.Contains(a)) is false)
                return new(new OperationFailedException(nameof(eventResponse.Data.EventDates), "One or more event date ID(s) does not exist"));

            return new(eventDateIds);
        }
    }
}
