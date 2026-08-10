using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Payments;

[Route("api/payments")]
[Authorize(policy: ApplicationAuthPolicy.HasAccessToEvent)]
public sealed class PaymentsController : AdminAppControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult), StatusCodes.Status201Created)]
    public async Task<ActionResult> MakePayment([FromBody] CreateOnsitePaymentDto model)
    {
        var response = await _paymentService.MakePaymentOnsite(model, CancellationToken.None);
        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }
}
