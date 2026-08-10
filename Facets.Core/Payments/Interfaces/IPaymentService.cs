using Facets.Core.Payments.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Core.Payments.Interfaces;

public interface IPaymentService
{
    Task<ResponseResult> MakePaymentOnsite(CreateOnsitePaymentDto model, CancellationToken cancellationToken);

    Task<ResponseResult<PaymentDto>> MakePaymentOnline(CreateOnlinePaymentDto model, CancellationToken cancellationToken);

}
