using Facets.Infrastructure.OnePay.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Infrastructure.OnePay.Interfaces;

public interface IOnePayService
{
    Task<ResponseResult<Gateway>> PaymentRequest(PaymentRequestDTO model, CancellationToken cancellationToken);
}
