using Facets.Core.Common.Dtos;
using Facets.Core.Common.Filters;
using Facets.Core.Common.Interfaces;
using Facets.Core.Events.DTOs;
using Facets.Core.Passes.DTOs;
using Facets.Core.Passes.Interfaces;
using Facets.SharedKernal.Helpers;
using Facets.SharedKernal.Models;
using Facets.SharedKernal.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using static Facets.Api.APIConstants;

namespace Facets.Api.Controllers;

[Route("api/lookups")]
public sealed class LookupsController : AdminAppControllerBase
{
    private readonly ILookupService _lookupService;
    private readonly IPassCategoryService _passCategoryService;

    public LookupsController(ILookupService lookupService, IPassCategoryService passCategoryService)
    {
        _lookupService = lookupService;
        _passCategoryService = passCategoryService;
    }

    [HttpGet("time-zones")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<TimeZoneModel>>), StatusCodes.Status200OK)]
    public ActionResult GetTimeZoneList(CancellationToken token)
    {
        var response = TimeZoneHelper.GetAllTimeZone();
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("Events")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetEventList([FromQuery] Paginator paginator, [FromQuery] EventLookupFilter filter, CancellationToken token)
    {
        var response = await _lookupService.GetEventList(paginator, filter, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("active-assigned-events")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetActiveAssignedEventList([FromQuery] Paginator paginator, CancellationToken token)
    {
        var response = await _lookupService.GetUserAssignedEventList(paginator, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("countries")]
    [AllowAnonymous]
    [OutputCache(PolicyName = OutputCachePolicyNames.CountryCachePolicy)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCountryList(CancellationToken token)
    {
        var response = await _lookupService.GetCountries(token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/pass-categories")]
    [OutputCache(PolicyName = OutputCachePolicyNames.PassCategoryCachePolicy)]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPassCategoryList([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] PassCategoryLookupFilter filter, CancellationToken token)
    {
        var response = await _lookupService.GetPassCategories(eventId, paginator, filter, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/registration-counter")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<RegistrationCounterLookUpDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRegistrationCounters([FromRoute] Guid eventId, [FromQuery] Paginator paginator, [FromQuery] RegistrationCounterLookupFilter filter, CancellationToken token)
    {
        var response = await _lookupService.GetRegistrationCounters(eventId, paginator, filter, token);

        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/pass-categories/{passCategoryId}")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPassCategoryById([FromRoute] Guid eventId, [FromRoute] Guid passCategoryId, CancellationToken token)
    {
        ResponseResult<PassCategoryDto> response = await _passCategoryService.GetPassCategoryById(eventId, passCategoryId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/assigned-users")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAssignedUsersToEvent([FromRoute] Guid eventId, CancellationToken token)
    {
        var response = await _lookupService.GetAssignedUsersToEvent(eventId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/pavilions")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<KeyValuePair<Guid, string>>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilionList([FromRoute] Guid eventId, CancellationToken token)
    {
        var response = await _lookupService.GetPavilions(eventId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }

    [HttpGet("events/{eventId}/pavilions/{pavilionId}/pavilion-sessions")]
    [ProducesResponseType(typeof(ResponseResult<IReadOnlyList<PavilionSessionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetPavilionSessionList([FromRoute] Guid eventId, [FromRoute] Guid pavilionId, CancellationToken token)
    {
        var response = await _lookupService.GetPavilionSessions(eventId, pavilionId, token);
        return response.Success ? Ok(response) : UnsuccessfullResponse(response);
    }
}
