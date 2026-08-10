using Facets.Core.Events.DTOs;
using Facets.Core.Events.Interfaces;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Interfaces;
using Facets.Core.Security.AuthPolicies;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facets.Api.Controllers.PublicSite;

[Route($"api/{APIConstants.APIGroup.Public}/events")]
[Authorize(policy: ApplicationAuthPolicy.PublicSiteUser)]
public sealed class EventsController : PublicAppControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PublicSiteEventSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetEvents([FromQuery] Paginator paginator, CancellationToken token)
    {
        var response = await _eventService.GetEventsForPublicSite(paginator, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("{eventId}/pass-categories/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseResult<PassCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetPassCategoryById([FromServices] IPassCategoryService passCategoryService, [FromRoute] Guid eventId, [FromRoute] Guid id, CancellationToken token)
    {
        ResponseResult<PassCategoryDto> response = await passCategoryService.GetPassCategoryById(eventId, id, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}


