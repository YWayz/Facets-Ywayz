using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Filters;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.V1.Passes;

[Route("api/events/{eventId}/pass-templates")]
public sealed class PassTemplatesController : AdminAppControllerBase
{
    private readonly IPassTemplateService _passTemplateService;

    public PassTemplatesController(IPassTemplateService passTemplateService)
    {
        _passTemplateService = passTemplateService;
    }

    [HttpPost]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.Manage)]
    [ProducesResponseType(typeof(ResponseResult<PassTemplateDto>), StatusCodes.Status201Created)]
    public async Task<ActionResult> CreatePassTemplate([FromRoute] Guid eventId, [FromBody] CreatePassTemplateDto model)
    {
        var response = await _passTemplateService.CreatePassTemplate(eventId, model, CancellationToken.None);

        return response.Success ? Created("", response) : UnsuccessfullResponse(response);
    }


    [HttpGet]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.Manage)]
    [ProducesResponseType(typeof(ResponseResult<PassTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPassTemplateById([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] PassTemplateFilter filter, CancellationToken token)
    {
        var response = await _passTemplateService.GetPassTemplates(paginator, eventId, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpPut("{id}")]
    [Authorize(policy: ApplicationAuthPolicy.PassCategoryPolicy.Manage)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdatePassTemplate([FromRoute] Guid eventId, Guid id, [FromBody] UpdatePassTemplateDto model)
    {
        var response = await _passTemplateService.UpdatePassTemplate(eventId, id, model, CancellationToken.None);

        return response.Success ? NoContent() : UnsuccessfullResponse(response);
    }
}
