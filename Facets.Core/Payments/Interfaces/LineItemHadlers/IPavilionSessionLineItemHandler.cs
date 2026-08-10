using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.PaymentCalculationHandlers;

namespace Facets.Core.Payments.Interfaces.LineItemHadlers;

internal interface IPavilionSessionLineItemHandler : IAsyncHandler<PaymentCalculationHandlerModel>
{
}
