using Facets.Api.Services;
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
    private readonly IPublicSiteOwnership _ownership;

    public PaymentsController(IPaymentService paymentService, IPublicSiteOwnership ownership)
    {
        _paymentService = paymentService;
        _ownership = ownership;
    }


    [HttpPost]
    [ProducesResponseType(typeof(ResponseResult<PaymentDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> MakePayment([FromBody] CreateOnlinePaymentDto model)
    {
        if (await _ownership.OwnsVisitor(model.VisitorId, CancellationToken.None) is false
            || await _ownership.OwnsRegistration(model.RegistrationId, CancellationToken.None) is false)
            return NotOwnedResponse("Registration", model.RegistrationId);

        var response = await _paymentService.MakePaymentOnline(model, CancellationToken.None);
        return response.Success ? Created(string.Empty, response) : UnsuccessfullResponse(response);
    }
}
