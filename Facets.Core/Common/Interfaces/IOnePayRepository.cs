using Facets.Core.Payments.Entities;

namespace Facets.Core.Common.Interfaces;
public interface IOnePayRepository : IBaseRepository
{
    OnePayRequestedPaymentResponseLog AddOnePayRequestedPaymentResponseLog(OnePayRequestedPaymentResponseLog entity);
}
