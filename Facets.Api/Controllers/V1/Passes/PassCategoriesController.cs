using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using static Facets.Api.APIConstants;

namespace Facets.Api.Controllers.V1.Passes;

[Route("api/events/{eventId}/pass-categories")]
public sealed class PassCategoriesController : AdminAppControllerBase
{
    private readonly IPassCategoryService _passCategoryService;
    private readonly IOutputCacheStore _cache;

    public PassCategoriesController(IPassCategoryService passCategoryService,  IOutputCacheStore cache)
    {
        _passCategoryService = passCategoryService;
        _cache = cache;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.CreatePassCategory)]
    [ProducesResponseType(typeof(ResponseResult<PassCategoryDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePassCategory([FromRoute] Guid eventId,
                                                       [FromBody] CreatePassCategoryDto model)
    {
        await _cache.EvictByTagAsync(OutputCachePolicyNames.PassCategoryCachePolicy, CancellationToken.None);

        var response = await _passCategoryService.CreatePassCategory(eventId, model, CancellationToken.None);

        return response.Success ? CreatedAtRoute(nameof(GetPassCategoryById), new { eventId = response.Data!.EventId, id = response.Data!.Id }, response) : UnsuccessfullResponse(response);
    }

    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.ViewPassCategory)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PassCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPassCategories([FromRoute] Guid eventId,
                                                      [FromQuery] Paginator paginator,
                                                      [FromQuery] PassCategoryFilter filter,
                                                      CancellationToken token)
    {
        var response = await _passCategoryService.GetPassCategories(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{id}", Name = nameof(GetPassCategoryById))]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.ViewPassCategory)]
    [ProducesResponseType(typeof(ResponseResult<PassCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPassCategoryById([FromRoute] Guid eventId, [FromRoute] Guid id, CancellationToken token)
    {
        ResponseResult<PassCategoryDto> response = await _passCategoryService.GetPassCategoryById(eventId, id, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.EditPassCategory)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassCategory([FromRoute] Guid eventId, 
                                                       [FromRoute] Guid id, 
                                                       [FromBody] UpdatePassCategoryDto model)
    {
        await _cache.EvictByTagAsync(OutputCachePolicyNames.PassCategoryCachePolicy, CancellationToken.None);

        var response = await _passCategoryService.UpdatePassCategory(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}/visitor-pass-category-type")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.EditPassCategory)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateVisitorPassCategoryType([FromRoute] Guid eventId, [FromRoute] Guid id, [FromBody] UpdateVisitorPassCategoryTypeDto model)
    {
        await _cache.EvictByTagAsync(OutputCachePolicyNames.PassCategoryCachePolicy, CancellationToken.None);

        var response = await _passCategoryService.UpdateVisitorPassCategoryType(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }

    [HttpDelete("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.DeletePassCategory)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeletePassCategory([FromRoute] Guid eventId, [FromRoute] Guid id)
    {
        await _cache.EvictByTagAsync(OutputCachePolicyNames.PassCategoryCachePolicy, CancellationToken.None);

        var response = await _passCategoryService.Delete(eventId, id, CancellationToken.None);
        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
