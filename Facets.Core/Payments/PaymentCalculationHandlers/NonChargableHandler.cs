using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.DTOs;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.PaymentCalculationHandlers;

internal sealed class NonChargableHandler : IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData>
{
    private IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData>? _next;

    public ResponseResult<ProcessedInvoiceData?> Handle(PaymentCalculationHandlerModel model)
    {
        var data = model.InternalCreateInvoiceDto.InvoicePaymentInfo;

        if (data.IsChargeable is true)
        {
            var response = _next?.Handle(model);
            return response!;
        }

        return new(new ProcessedInvoiceData(PassCategorySettingId: data.PassCategorySettingId,
                                            PassCategoryId: data.PassCategoryId,
                                            IsChargeable: data.IsChargeable,
                                            DiscountType: DiscountType.NoDiscount,
                                            AmountPerLineItem: 0M));
    }

    public IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData> SetNextHandler(IHandler<PaymentCalculationHandlerModel, ProcessedInvoiceData> next)
    {
        _next = next;
        return _next;
    }
}
