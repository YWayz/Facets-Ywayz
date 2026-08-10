using Facets.Core.Common.Interfaces;
using Facets.Core.Events.Interfaces;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Payments.Interfaces.LineItemHadlers;
using Facets.Core.Payments.PaymentCalculationHandlers;
using Facets.SharedKernal.Responses;
using static Facets.SharedKernal.AppEnums;

namespace Facets.Core.Payments.Services.LineItemHandlers;

internal sealed class PavilionSessionLineItemHandler : IPavilionSessionLineItemHandler
{
    private readonly IPavilionRepository _pavilionRepository;
    private readonly IPassCategoryRepository _passCategoryRepository;
    private IAsyncHandler<PaymentCalculationHandlerModel>? _next;

    public PavilionSessionLineItemHandler(IPavilionRepository pavilionRepository, IPassCategoryRepository passCategoryRepository)
    {
        _pavilionRepository = pavilionRepository;
        _passCategoryRepository = passCategoryRepository;
    }
    public async Task<ResponseResult> Handle(PaymentCalculationHandlerModel model)
    {
        var visitorPavilionSessionAttendanceScheduleIds = model.InternalCreateInvoiceDto
                                                               .InvoiceLineItemSets[InvoiceLineItemType.VisitorPavilionSessionAttendance];

        var pavilionRates = await _passCategoryRepository.GetPavilionRatesForPassCategoryByAttendanceScheduleId(model.InternalCreateInvoiceDto
                                                                                                                     .InvoicePaymentInfo
                                                                                                                     .PassCategoryId,
                                                                                                        visitorPavilionSessionAttendanceScheduleIds);

        foreach (var visitorPavilionSessionAttendanceScheduleId in visitorPavilionSessionAttendanceScheduleIds)
        {
            var visitorPavilionRate = pavilionRates.First(f => f.VisitorPavilionSessionId == visitorPavilionSessionAttendanceScheduleId);

            model.AddLineItem(new(visitorPavilionSessionAttendanceScheduleId,
                                  visitorPavilionRate.PavilionRate,
                                  InvoiceLineItemType.VisitorPavilionSessionAttendance));
        }

        var response = await (_next?.Handle(model) ?? Task.FromResult(new ResponseResult()));
        return response;
    }

    public IAsyncHandler<PaymentCalculationHandlerModel> SetNextHandler(IAsyncHandler<PaymentCalculationHandlerModel> next)
    {
        _next = next;
        return _next;
    }
}
