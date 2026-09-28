using Facets.Infrastructure.OnePay.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Infrastructure.OnePay.Interfaces;

public interface IOnePayService
{
    Task<ResponseResult<Gateway>> PaymentRequest(PaymentRequestDTO model, CancellationToken cancellationToken);

    /// <summary>
    /// Asks OnePay directly whether a transaction was paid. Use this to verify webhook
    /// notifications instead of trusting their payload.
    /// </summary>
    Task<ResponseResult<OnePayTransactionStatusDto>> GetTransactionStatus(string onePayTransactionId, CancellationToken cancellationToken);
}
