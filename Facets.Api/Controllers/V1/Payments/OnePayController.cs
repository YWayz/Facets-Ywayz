using Facets.Infrastructure.OnePay.DTOs;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Payments;

[Route("api/onepay")]
[ApiController]
public sealed class OnePayController : AppControllerBase
{
    private readonly IOnePayService _onePayService;

    public OnePayController(IOnePayService onePayService)
    {
        _onePayService = onePayService;
    }

    [HttpPost("request-payment")]
    [ProducesResponseType(typeof(ResponseResult<Gateway>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PaymentRequest([FromBody] PaymentRequestDTO model)
    {
        var response = await _onePayService.PaymentRequest(model, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }
}
