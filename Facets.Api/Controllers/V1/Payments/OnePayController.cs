using Facets.Api.Services;
using Facets.Infrastructure.OnePay.DTOs;
using Facets.Infrastructure.OnePay.Interfaces;
using Facets.SharedKernal.Exceptions;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Payments;

[Route("api/onepay")]
[ApiController]
public sealed class OnePayController : AppControllerBase
{
    private readonly IOnePayService _onePayService;
    private readonly IPublicSiteOwnership _ownership;

    public OnePayController(IOnePayService onePayService, IPublicSiteOwnership ownership)
    {
        _onePayService = onePayService;
        _ownership = ownership;
    }

    [HttpPost("request-payment")]
    [ProducesResponseType(typeof(ResponseResult<Gateway>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PaymentRequest([FromBody] PaymentRequestDTO model)
    {
        // Visitors may only start a payment for their own invoice.
        if (_ownership.IsPublicSiteUser && await _ownership.OwnsInvoice(model.InvoiceId, CancellationToken.None) is false)
            return UnsuccessfullResponse(new ResponseResult(new NotFoundException(nameof(model.InvoiceId), "Invoice", model.InvoiceId)));

        var response = await _onePayService.PaymentRequest(model, CancellationToken.None);

        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }
}
