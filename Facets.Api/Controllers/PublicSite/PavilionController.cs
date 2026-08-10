using Facets.Core.Events.Filters;
using Facets.Core.Events.Interfaces;
using Facets.Core.Events.Specs;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/events")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class PavilionController : PublicAppControllerBase
{
    private readonly IPavilionService _pavilionService;

    public PavilionController(IPavilionService pavilionService)
    {
        this._pavilionService = pavilionService;
    }

    [HttpGet("{eventId}/pavilions")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PavilionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilions([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] PavilionFilter filter, CancellationToken token)
    {
        var response = await _pavilionService.GetPavilions(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}