using Facets.Infrastructure.OnePay.DTOs;
using Facets.SharedKernal.Responses;

namespace Facets.Infrastructure.OnePay.Interfaces;

internal interface IOnePayAPIService
{
    internal Task<ResponseResult<OnePaymentRequestedPaymentAPIResponse>> RequestPayment(OnePayPaymentRequestDto newPaymentRequest);

    internal Task<ResponseResult<OnePayTransactionStatusDto>> GetTransactionStatus(string onePayTransactionId, CancellationToken cancellationToken);
}
