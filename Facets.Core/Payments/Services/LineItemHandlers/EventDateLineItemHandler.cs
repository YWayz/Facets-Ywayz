using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Interfaces.LineItemHadlers;
using Facets.Core.Payments.PaymentCalculationHandlers;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Services.LineItemHandlers;

internal sealed class EventDateLineItemHandler : IEventDateLineItemHandler
{
    private IAsyncHandler<PaymentCalculationHandlerModel>? _next;

    public async Task<ResponseResult> Handle(PaymentCalculationHandlerModel model)
    {
        var paymentAmountHandler = new NonChargableHandler();
        paymentAmountHandler.SetNextHandler(new OnlineRegistrationDiscountedRateHandler())
                            .SetNextHandler(new EntireEventDiscountedRateHandler())
                            .SetNextHandler(new EarlyRegistrationDiscountedRateHandler())
                            .SetNextHandler(new StandardRateHandler());

        var eventDateRateResponse = paymentAmountHandler.Handle(model);

        if (eventDateRateResponse.Success is false) return new(eventDateRateResponse.Errors);

        foreach (var visitorAttendanceScheduleId in model.InternalCreateInvoiceDto.InvoiceLineItemSets[InvoiceLineItemType.VisitorEventAttendance])
        {
            model.AddLineItem(new(visitorAttendanceScheduleId,
                                  eventDateRateResponse.Data!.AmountPerLineItem,
                                  InvoiceLineItemType.VisitorEventAttendance));
        }

        model.ProcessedInvoiceData = eventDateRateResponse.Data!;

        var response = await (_next?.Handle(model) ?? Task.FromResult(new ResponseResult()));

        return response;
    }

    public IAsyncHandler<PaymentCalculationHandlerModel> SetNextHandler(IAsyncHandler<PaymentCalculationHandlerModel> next)
    {
        return _next = next;
    }
}
