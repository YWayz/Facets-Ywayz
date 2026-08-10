using Facets.Core.Payments.DTOs;
using Facets.Core.Payments.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/payments")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class PaymentsController : PublicAppControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }


    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<PaymentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> MakePayment([FromBody] CreateOnlinePaymentDto model)
    {
        var response = await _paymentService.MakePaymentOnline(model, CancellationToken.None);
        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }
}
