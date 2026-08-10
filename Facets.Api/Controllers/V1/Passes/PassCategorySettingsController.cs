using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Passes;

[Route("api/events/{eventId}/pass-categories")]
public sealed class PassCategorySettingsController : AdminAppControllerBase
{
    private readonly IPassCategoryService _passCategoryService;

    public PassCategorySettingsController(IPassCategoryService passCategoryService)
    {
        _passCategoryService = passCategoryService;
    }

    [HttpGet("rates")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.ViewPassRate)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PassCategoryRateDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPassCategoryRates([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] PassCategorySettingsFilter filter, CancellationToken token)
    {
        var response = await _passCategoryService.GetPassCategoryRates(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{passCategoryId}/rates/{id}", Name = nameof(GetPassCategoryRateById))]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.ViewPassRate)]
    [ProducesResponseType(typeof(ResponseResult<PassCategoryRateDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPassCategoryRateById([FromRoute] Guid eventId, [FromRoute] Guid passCategoryId, [FromRoute] Guid id, CancellationToken token)
    {
        ResponseResult<PassCategoryRateDetailDto> response = await _passCategoryService.GetPassCategoryRateById(eventId, passCategoryId, id, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{passCategoryId}/rates/{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.EditPassRate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassCategoryRate([FromRoute] Guid eventId, [FromRoute] Guid passCategoryId, [FromRoute] Guid id, [FromBody] UpdatePassCategoryRateDto model)
    {
        var response = await _passCategoryService.UpdatePassCategoryRate(eventId, passCategoryId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{passCategoryId}/rates/{id}/status")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.EditPassRate)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateIsChargeableStatus([FromRoute] Guid eventId, [FromRoute] Guid passCategoryId, [FromRoute] Guid id, [FromBody] UpdatePassCategoryRateIsCharegableStatusDto model)
    {
        var response = await _passCategoryService.UpdateIsChargeableStatus(eventId, passCategoryId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
