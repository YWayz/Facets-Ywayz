using Facets.Core.Common.Interfaces;
using Facets.Core.Payments.Entities;

namespace Facets.Core.Payments.Interfaces;

public interface IPaymentRepository : IBaseRepository
{
    Payment Add(Payment payment);
}
