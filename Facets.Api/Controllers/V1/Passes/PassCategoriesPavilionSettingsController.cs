using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Passes;

[Route("api/events/{eventId}/pavilions/{pavilionId}/pass-categories-pavilion-settings")]
public class PassCategoriesPavilionSettingsController : AppControllerBase
{
    private readonly IPassCategoryService _passCategoryService;

    public PassCategoriesPavilionSettingsController(IPassCategoryService passCategoryService)
    {
        _passCategoryService = passCategoryService;
    }

    [HttpPut("rates")]
    [Authorize(policy: ApplicationAuthPolicy.PavilionPolicy.EditPassPavilionRate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassCategoryPavilionRate([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, [FromBody] UpdatePassCategoryPavilionRateDto model)
    {
        var response = await _passCategoryService.UpdatePassCategoryPavilionRate(eventId, pavilionId, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
