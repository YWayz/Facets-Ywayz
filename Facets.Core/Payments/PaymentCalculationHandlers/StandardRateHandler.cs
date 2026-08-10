using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.DTOs;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.PaymentCalculationHandlers;

internal sealed class StandardRateHandler : IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData>
{
    private IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData>? _next;

    public ResponseResult<ProcessedInvoiceData?> Handle(PaymentCalculationHandlerModel model)
    {
        var data = model.InternalCreateInvoiceDto.InvoicePaymentInfo;

        return new(new ProcessedInvoiceData(PassCategorySettingId: data.PassCategorySettingId,
                                            PassCategoryId: data.PassCategoryId,
                                            IsChargeable: data.IsChargeable,
                                            DiscountType: DiscountType.NoDiscount,
                                            AmountPerLineItem: data.Rate));
    }

    public IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData> SetNextHandler(IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData> next)
    {
        _next = next;
        return _next;
    }
}
